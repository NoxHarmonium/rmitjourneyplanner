/*---------------------------------------------------------------------------*/
/* Approximation of the Least Contributor                                    */
/*---------------------------------------------------------------------------*/
/* This program calculates for a given set of points in R^d a point, which   */
/* has a hypervolume contribution very close to the minimal contribution of  */
/* any point. In [BF09] we prove that for given delta,epsilon>0, the         */
/* returned solution has a contribution at most (1+epsilon) times the        */
/* minimal contribution with probability at least (1-delta).  More details   */
/* can be found in [BF09].                                                   */
/* This implementation may use the exact hypervolume algorithms HSO          */
/* by Zitzler (available at                                                  */
/* ftp://ftp.tik.ee.ethz.ch/pub/people/zitzler/hypervol.c) and BR by         */
/* Beume and Rudolph (available upon request from the two) to speed up on    */
/* small subcases.                                                           */
/* If you do not use HSO and/or BR, you should disable the flags             */
/* USE_EXACT_HSO and/or USE_EXACT_BR.                                        */
/* Also note that the algorithm assumes that no point is dominated by        */
/* another, meaning that you should run a test for domination before         */
/* invocing the algorithm.                                                   */
/*                                                                           */
/* [BF09] K. Bringmann, T. Friedrich.  Approximating the least hypervolume   */
/*        contributor: NP-hard in general, but fast in practice.             */
/*        Proc. of the 5th International Conference on Evolutionary          */
/*        Multi-Criterion Optimization (EMO 2009), Nantes, France,           */
/*        Vol. 5467 of Lecture Notes in Computer Science, pages 6-20,        */
/*        Springer-Verlag, 2009.                                             */
/*                                                                           */
/*---------------------------------------------------------------------------*/
/* Karl Bringmann                                                            */
/* Saarland University, Germany                                              */
/* and                                                                       */
/* Tobias Friedrich                                                          */
/* Max-Planck-Institut für Informatik, Saarbrücken, Germany                  */
/* (c)2010                                                                   */
/*---------------------------------------------------------------------------*/

#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <time.h>
#include <limits.h>
#include <float.h>

// incorporated exact algorithms: 
#include "HSO.h"
#include "BR.h"

// flags for using the exact algorithms:
#define USE_EXACT
#define USE_EXACT_EXP
#define USE_EXACT_HSO  // activate this flag, if an implementation of HSO is available
#define USE_EXACT_BR  // activate this flag, if an implementation of BR is available

#define ERROR(x)  fprintf(stderr, x), fprintf(stderr, "\n"), exit(1)


/************************************/
/* Constants used by the algorithm  */
/************************************/

const double START_DELTA = 0.1;  
  // Delta to start with
const double MULTIPLIER_DELTA = 0.775;  
  // decrease factor of Delta
const double MINIMUM_DELTA_MULTIPLIER = 0.2;  
  // how much more do we sample the current minimum?
const long long MAXNUMSAMPLES = LLONG_MAX;  
  // upper bound on the number of samples
const double GAMMA = 0.25;
  // how do we distribute the probabilities?


/********************/
/* helper functions */
/********************/

typedef struct {
  int index;
  double volume;
} PAIR_INDEX_VOLUME;

double min_dbl(double a, double b) {
  if (a < b) return a;
  return b;
}

double sqr(double a) {
  return a*a;
}


/*******************************************************/
/* exact algorithms:                                   */
/* estimation of their runtime                         */
/* an implementation of the inclusion/exclusion mehtod */
/* calls to HSO and BR                                 */
/*******************************************************/

double exponentialExactHypervolumeRecursion( double * front[], int n, int noObjectives, 
      int sign, int took, double * point ) {
/* recursively implements the inclusion/exclusion principle to compute 
   the hypervolume of the given front by an O(2^n)-algorithm
   
   front        - set of points which hypervolume we want to compute
   n            - running variable of the recursion, the current point
   noObjectives - number of dimensions
   sign         - sign of the current chosen set in the inclusion/exclusion equality
   took         - number of elements in the current chosen set
   point        - point[j] is the minimal j-th coordinate of any point in the current chosen set
*/
  int k;
  if (n < 0) {
    double res = 1.;
    for (k=0; k<noObjectives; k++) res *= point[k];
    if (took > 0) return sign * res;
    else return 0.;
  }
  else {
    // do not include point n:
    double res = exponentialExactHypervolumeRecursion( front, n-1, noObjectives, sign, took, point );
    // include point n: 
    double * nxtpoint = (double*) malloc(noObjectives * sizeof(double));
    for (k=0; k<noObjectives; k++) {
      nxtpoint[k] = min_dbl( point[k], front[n][k] );
    }
    // add up:
    res += exponentialExactHypervolumeRecursion( front, n-1, noObjectives, - sign, took+1, nxtpoint );
    free(nxtpoint);
    return res;
  }
}

double exponentialExactHypervolume( double * front[], int noPoints, int noObjectives ) {
/* implements the inclusion/exclusion principle to compute 
   the hypervolume of the given front by an O(2^n)-algorithm
   
   front        - set of points which hypervolume we want to compute
   noPoints     - number of points in the front
   noObjectives - number of dimensions
*/
  int k, i;
  
  double * point = (double*) malloc(noObjectives * sizeof(double));
  
  // initialize "point" as a bounding box of all given boxes:
  for (k=0; k<noObjectives; k++) {
    point[k] = front[0][k];
    for (i=1; i<noPoints; i++)
      if (front[i][k] > point[k]) point[k] = front[i][k];
  }
  
  // start the recursive call:
  double res = exponentialExactHypervolumeRecursion( front, noPoints - 1, noObjectives, -1, 0, point );
  
  free(point);
  
  return res;
}

double binomial( int n, int k ) {
/* computes the binomial coefficient. 
*/
  if (k > n/2) return binomial(n, n-k);
  else if (k == 0) return 1.;
  else return ( n * binomial( n-1, k-1 ) ) / k;
}

double runtimeExp(int n, int d) {
/* Estimatation of the runtime (number of operations counted by noOps) 
   of the O(2^n)-algorithm on noPoints = n and noObjectives = d. 
*/
  return 0.1 * exp( n * log(2) ) * d;
}

double runtimeHSO(int n, int d) {
/* Estimatation of the runtime (number of operations counted by noOps) of HSO.
*/
  if (n < 10) return runtimeHSO( 10, d );
  return 0.00001 *d*d* n * binomial( n + d - 2, d - 1 );
}

double runtimeBR(int n, int d) {
/* Estimatation of the runtime (number of operations counted by noOps) 
   of the algorithm by Beume & Rudolph.
*/
  if (n < 10) return runtimeBR( 10, d );
  return 0.03 *d*d* exp( log(n) * d * 0.5 );
}

double runtimeExactHypervolume( int n, int d ) {
/* Estimation of the minimal runtime of any of the exact algorithms incorporated.
*/
  double res = DBL_MAX;
  #ifdef USE_EXACT_EXP
  res = min_dbl( res, runtimeExp(n, d) );
  #endif
  #ifdef USE_EXACT_HSO
  res = min_dbl( res, runtimeHSO(n, d) );
  #endif
  #ifdef USE_EXACT_BR
  res = min_dbl( res, runtimeBR(n, d) );
  #endif
  return res;
}

double exactHypervolume( int i, int * start_infl, int * influencing, 
         double * boundBoxLower, double * boundBoxVol, double * front[], int noObjectives ) {
/* generates a hypervolume problem which solution corresponds to the contribution 
   of points i. then runs the (estimated) fastest exact hypervolume algorithm.
   
   i              - point of which we want to compute the contribution
   start_infl     - the starting indices in the influencing array
   influencing    - for each point: the set of points influencing its bounding box
   boundBoxLower  - the lower ends of the bounding boxes
   boundBoxVol    - the volumes of the bounding boxes
   front          - set of points
   noObjectives   - number of dimensions
*/
  int j, k;
  
  // if no influencing box exist, return the volume of the bounding box:
  if (start_infl[i+1] - start_infl[i] == 0) {
    return boundBoxVol[i];
  }
  
  // generate the hypervolume instance of all boxes reduced to the bounding box of i:
  double ** tmpfront = (double**) malloc((start_infl[i+1] - start_infl[i]) * sizeof(double *));
  for (j = 0; j < start_infl[i+1] - start_infl[i]; j++) {
    tmpfront[j] = (double*) malloc(noObjectives * sizeof(double));
  }
  int n = start_infl[i+1] - start_infl[i];
  int d = noObjectives;
  for (j=0; j < n; j++) {
    for (k=0; k < noObjectives; k++)
      tmpfront[j][k] = min_dbl(front[influencing[start_infl[i] + j]][k], front[i][k]) - boundBoxLower[i*noObjectives + k];
  }
  
  // determine the algo to use and run it:
  double res = 0.;
  int algoToUse = 0;
  double time = 1E50;
  #ifdef USE_EXACT_HSO
  if (time > runtimeHSO(n, d)) {
    time = runtimeHSO(n, d);
    algoToUse = 1;
  }
  #endif
  #ifdef USE_EXACT_EXP
  if (time > runtimeExp(n, d)) {
    time = runtimeExp(n, d);
    algoToUse = 2;
  }
  #endif
  #ifdef USE_EXACT_BR
  if (time > runtimeBR(n, d)) {
    time = runtimeBR(n, d);
    algoToUse = 3;
  }
  #endif
  #ifdef USE_EXACT_HSO
  if (algoToUse == 1) {
    res = CalculateHypervolume( tmpfront, n, d );  // <-- insert the name of your HSO method here
  }
  #endif
  #ifdef USE_EXACT_EXP
  if (algoToUse == 2) {
    res = exponentialExactHypervolume( tmpfront, n, d );
  }
  #endif
  #ifdef USE_EXACT_BR
  if (algoToUse == 3) {
    res = BR( tmpfront, n, d );  // <-- insert the name of your BR method here
  }
  #endif
  
  for (j = 0; j < n; j++) {
    free(tmpfront[j]);
  }
  free(tmpfront);
  
  return boundBoxVol[i] - res;
}


/********************************/
/* the algorithm:               */
/* sampling                     */
/* building the data structures */
/* the main method              */
/********************************/

// local variables and arrays:
double ** _alc_front;
  // set of points of which we want to compute the least contributor
int _alc_noPoints;
  // number of points in the front
int _alc_noObjectives;
  // number of dimensions
int * _alc_influencing; 
  // for each point: the set of points influencing its bounding box
int * _alc_start_infl;
  // the starting indices in the _alc_influencing array
double * _alc_boundBoxVol;
  // the volumes of the bounding boxes
double * _alc_boundBoxLower;
  // the lower ends of the bounding boxes
PAIR_INDEX_VOLUME * _alc_qsortarray;
  // helping array for sorting the influencing boxes by volume
int * _alc_actives;
  // contains the indices of all points still in the race
long long * _alc_noSamples;
  // for each point: the number of sample points drawn
long long * _alc_noSuccSamples;
  // for each point: the number of sample points drawn, that lay inside the contribution space
long long * _alc_noOps;
  // for each point: the number of comparisons we did when sampling in it,
  // i.e., an estimation of the runtime we used for it
double * _alc_approx;
  // an approximation of the contribution of each point
double _alc_logfactor;
  // factor used to compute current error threshold of a point


double rand_dbl() {
/* The randomness used by the sampling algorithm. 
*/
  double r1 = (double) rand();
  double r2 = (double) rand();
  double r3 = (double) rand();
  double m = (double) RAND_MAX + 1.;
  return (r1 + ( (r2 + (r3/m) ) / m) ) / m;
}

int Sample( int i ) {
/* samples a random point inside the bounding box of point i.
*/
  int j, k;
  // get a random point:
  double * point = (double*) malloc(_alc_noObjectives * sizeof(double));
  for (j=0; j<_alc_noObjectives; j++) {
    point[j] = _alc_boundBoxLower[i*_alc_noObjectives + j] + (_alc_front[i][j] - _alc_boundBoxLower[i*_alc_noObjectives + j]) * rand_dbl();
  }
  
  // check, whether it is included in some influencing box:
  int successful = 1;
  for (j=_alc_start_infl[i]; j<_alc_start_infl[i+1]; j++) {
    // is "point" inside the box with index "_alc_influencing[j]" ?
    int inside = 1;
    for (k=0; k<_alc_noObjectives; k++) {
      if (_alc_front[_alc_influencing[j]][k] < point[k]) { inside = 0; break; }
    }
    _alc_noOps[i] += k+1;
    if (inside) {
      successful = 0;
      break;
    }
  }
  
  free(point);
  
  return successful;
}

int Cmp_by_volume_desc( const void * X, const void * Y ) {
  PAIR_INDEX_VOLUME * x = (PAIR_INDEX_VOLUME*) X;
  PAIR_INDEX_VOLUME * y = (PAIR_INDEX_VOLUME*) Y;
  if (x->volume > y->volume) return -1;
  else if (x->volume < y->volume) return 1;
  else return 0;
}

int Compute_bounding_boxes() {
/* builds the arrays "_alc_boundBoxLower", "_alc_boundBoxVol" and "_alc_start_infl":
   _alc_boundBoxLower: stores for each point the coordinates of the lower left point of the 
       bounding box of that point
   _alc_boundBoxVol: stores for each point the volume of its bounding box
   _alc_start_infl: contains for each point the number of points inside its bounding box, i.e.,
       the number of influencing boxes. The accumulated sum for all previous points is stored.
*/
  int i, j, k, l;
  _alc_start_infl[0] = 0;
  for (i=0; i<_alc_noPoints; i++) {
    for (k=0; k<_alc_noObjectives; k++) _alc_boundBoxLower[i*_alc_noObjectives+k] = 0.;
    for (j=0; j<_alc_noPoints; j++) {
      // each point k with one coordinate lower than the corresponding 
      // coordinate of i and all other coordinates greater than the ones of i
      // cuts something off the bounding box:
      int noOfGreaterCoords = 0; int w = 0;
      for (l=0; l<_alc_noObjectives; l++) {
        if (_alc_front[j][l] < _alc_front[i][l]) {
          noOfGreaterCoords++;
          if (noOfGreaterCoords == 2) break;
          w = l;
        }
      }
      if (noOfGreaterCoords == 1 && _alc_boundBoxLower[i*_alc_noObjectives+w] < _alc_front[j][w]) {
        _alc_boundBoxLower[i*_alc_noObjectives+w] = _alc_front[j][w];
      }
    }
    // determine boundBoxVol:
    _alc_boundBoxVol[i] = 1.;
    for (k=0; k<_alc_noObjectives; k++) {
      _alc_boundBoxVol[i] *= _alc_front[i][k] - _alc_boundBoxLower[_alc_noObjectives*i+k];
    }
    
    // determine the number of influencing boxes:
    int ni = 0;
    for (k=0; k<_alc_noPoints; k++) {
      if (k != i) {
        int isinfluencing = 1;
        for (l=0; l<_alc_noObjectives; l++)
          if (_alc_front[k][l] <= _alc_boundBoxLower[_alc_noObjectives*i+l]) { isinfluencing = 0; break; }
        ni += isinfluencing;
      }
    }
    _alc_start_infl[i+1] = _alc_start_infl[i] + ni;
  }
  return 0;
}

int Build_influencing_array( int i ) {
/* builds the array "_alc_influencing" (for point i), which includes the sets of 
   influencing boxes for each point.
   
   i - index of the current point
*/
    int k, l;
    int s = 0;
    // determine th influencing points: 
    for (k=0; k<_alc_noPoints; k++) {
      if (k != i) {
        int isinfluencing = 1;
        for (l=0; l<_alc_noObjectives; l++)
          if (_alc_front[k][l] <= _alc_boundBoxLower[_alc_noObjectives*i+l]) { isinfluencing = 0; break; }
        if (isinfluencing) {
          _alc_qsortarray[s].index = k;
          double vol = 1.;
          // compute the volume overlapped by k inside the bounding box of i
          for (l=0; l<_alc_noObjectives; l++)
            vol *= min_dbl(_alc_front[k][l],_alc_front[i][l]) - _alc_boundBoxLower[_alc_noObjectives*i+l];
          _alc_qsortarray[s].volume = vol;
          s++;
        }
      }
    }
    
    // sort these points according to the volume they overlap inside the bounding box
    qsort( _alc_qsortarray, s, sizeof(PAIR_INDEX_VOLUME), Cmp_by_volume_desc );
    
    for (k=0; k<s; k++) {
      _alc_influencing[_alc_start_infl[i] + k] = _alc_qsortarray[k].index;
    }
    return 0;
}

int Sample_till_delta( int i, double delta, int R ) {
/* this method samples in the bounding box of point i until we reach the error threshold 
   delta or until we can afford computing the contribution exactly.
   
   i             - index of the current point
   delta         - the error threshold we want to reach
*/
  // we only sample if we did not already computed the contribution exactly:
  if (_alc_noSamples[i] < MAXNUMSAMPLES) {
    #ifdef USE_EXACT
    // can we afford computing the contribution exactly?
    if (_alc_noOps[i] >= runtimeExactHypervolume( _alc_start_infl[i+1] - _alc_start_infl[i], _alc_noObjectives )) {
      _alc_noSamples[i] = MAXNUMSAMPLES;
      _alc_noSuccSamples[i] = MAXNUMSAMPLES;
      _alc_approx[i] = exactHypervolume( i, _alc_start_infl, _alc_influencing, _alc_boundBoxLower, 
                                    _alc_boundBoxVol, _alc_front, _alc_noObjectives );
    }
    else {
    #endif
      // sample until we reach the error threshold delta:
      double thresh = 0.5 * ((1. + GAMMA) * log(R) + _alc_logfactor) * sqr( _alc_boundBoxVol[i] / delta);
      for (; _alc_noSamples[i] == 0 || 
             ((double)_alc_noSamples[i]) < thresh; 
             _alc_noSamples[i]++) {
        if (Sample(i)) {
          _alc_noSuccSamples[i]++;
        }
      }
      _alc_approx[i] = _alc_boundBoxVol[i] * ((double) _alc_noSuccSamples[i]) / ((double) _alc_noSamples[i]);
    #ifdef USE_EXACT
    }
    #endif
  }
  return 0;
}

double Delta( int i, int R ) {
/* computes the current error threshold point i fulfils 
   
   i - point of which we want to compute the current Delta 
*/
  return sqrt( 0.5 * ((1. + GAMMA) * log(R) + _alc_logfactor) / _alc_noSamples[i] ) * _alc_boundBoxVol[i];
}

int ApproximateLeastContributor(double * _front[], int _noPoints, 
                                int _noObjectives, double failureProb, 
                                double epsAbort, double * approxVol) {
/* the main method. here we perform the race, starting sampling and checking
   the deletion and abortion criterias in every round.
   
   _front        - the set of points of which we want to compute the least contributor
   _noPoints     - number of points in the front
   _noObjectives - number of dimensions
   failureProb   - probability, with which we might return a wrong result (i.e., no point
                   which contributes at most (1 + epsAbort) * minimalContribution).
                   10^-6 and even 10^-12 are reasonable values.
   epsAbort      - the error that the contribution of the returned point might have, i.e.,
                   we may return a point with contribution at most 
                   (1 + epsAbort) * minimalContribution.
                   10^-1 or up to 10^-2 are reasonable values. for "nice" pointsets
                   this value is not taken into account, so that we can make it arbitrary small.
   approxVol     - if this is not NULL, an approximation of the contribution of the result 
                   will be saved in this variable
*/
  _alc_front = _front;
  _alc_noPoints = _noPoints;
  _alc_noObjectives = _noObjectives;
  
  srand(time(0));
  int i, j;
  
  // allocation of the used arrays:
  _alc_start_infl = (int*) malloc((_alc_noPoints + 1) * sizeof(int));
  _alc_boundBoxVol = (double*) malloc(_alc_noPoints * sizeof(double));
  _alc_boundBoxLower = (double*) malloc(_alc_noPoints * _alc_noObjectives * sizeof(double));
  _alc_qsortarray = (PAIR_INDEX_VOLUME*) malloc(_alc_noPoints * sizeof(PAIR_INDEX_VOLUME));
  _alc_actives = (int*) malloc(_alc_noPoints * sizeof(int));
  _alc_noSamples = (long long int*) malloc(_alc_noPoints * sizeof(long long));
  _alc_noSuccSamples = (long long int*) malloc(_alc_noPoints * sizeof(long long));
  _alc_noOps = (long long int*) malloc(_alc_noPoints * sizeof(long long));
  _alc_approx = (double*) malloc(_alc_noPoints * sizeof(double));
  
  // build the used data structures:
  
  Compute_bounding_boxes();
  
  _alc_influencing = (int*) malloc(_alc_start_infl[_alc_noPoints] * sizeof(int));
  
  for (i=0; i<_alc_noPoints; i++) {
    Build_influencing_array(i);
  }
  
  // initialize the approximation values:
  int N = _alc_noPoints;  // N = number of points still in the race
  for (i=0; i<_alc_noPoints; i++) {
    _alc_actives[i] = i;
    _alc_noSamples[i] = 0;
    _alc_noOps[i] = 0;
    _alc_noSuccSamples[i] = 0;
    _alc_approx[i] = 0.;
  }
  
  // start the race:
  double maxBBvol = 0.;
  for (i=0; i<_alc_noPoints; i++) if (_alc_boundBoxVol[i] > maxBBvol) maxBBvol = _alc_boundBoxVol[i];
  double delta = START_DELTA * maxBBvol;
  _alc_logfactor = log( 2. * _alc_noPoints * (1. + GAMMA) / (failureProb * GAMMA) );
  double minapprox = DBL_MAX;
  int mini = 0;
  int R = 0;
  while (N > 1) {
    R++;
    minapprox = DBL_MAX;
    mini = 0;
    // sample for all active points up to error delta:
    for (i=0; i<N; i++) {
      j = _alc_actives[i];
      Sample_till_delta(j, delta, R);
      if (_alc_approx[j] < minapprox) {
        minapprox = _alc_approx[j];
        mini = i;
      }
    }
    // sample for the current minimal box up to error MIN_DELTA_MULTIPLIER * delta:
    if (N > 2) {
      Sample_till_delta(_alc_actives[mini], MINIMUM_DELTA_MULTIPLIER * delta, R);
    }
    // adjust minimum:
    minapprox = DBL_MAX;
    for (i=0; i<N; i++) {
      j = _alc_actives[i];
      if (_alc_approx[j] < minapprox) {
        minapprox = _alc_approx[j];
        mini = i;
      }
    }
    
    // delete the points that fulfil the deletion criterion:
    for (i=0; i<N; i++) {
      j = _alc_actives[i];
      // deletion criterion:
      while (_alc_approx[j] - minapprox > Delta(j,R) + Delta(_alc_actives[mini],R)) {
        // delete box i:
        _alc_actives[i] = _alc_actives[N-1];
        N--;
        if (i >= N) break;
        j = _alc_actives[i];
      }
    }
    if (N == 1) break;
    
    // check, whether the remaining points fulfil the abortion criterion:
    double maxQ = 0.;
    int k = _alc_actives[mini];
    for (i=0; i<N; i++) {
      if (i != mini) {
        j = _alc_actives[i];
        double nom = minapprox + Delta(k,R);
        double den = _alc_approx[j] - Delta(j,R);
        if (den <= 0.) maxQ = DBL_MAX;
        else if (nom / den > maxQ) maxQ = nom / den;
      }
    }
    if (maxQ < 1. + epsAbort) {
      // abort:
      break;
    }
    
    // decrease delta:
    delta *= MULTIPLIER_DELTA;
  }
  printf("R = %d\n", R);
  
  int res = _alc_actives[mini];
  if (approxVol != NULL)
    *approxVol = minapprox;
  
  free(_alc_start_infl);
  free(_alc_boundBoxVol);
  free(_alc_boundBoxLower);
  free(_alc_qsortarray);
  free(_alc_influencing);
  free(_alc_actives);
  free(_alc_noSamples);
  free(_alc_noSuccSamples);
  free(_alc_noOps);
  free(_alc_approx);
  
  return res;
}

/**********************************************************/
/* helper functions for file handling and building fronts */
/**********************************************************/

int  ReadFront(double  **frontPtr[], FILE  *file, int  noObjectives, int noPoints) {
/* Reads a set of points from a file 
*/
  int     i;
  double  value;
  int     T;
  
  /* allocate memory */
  /* read data */
  for(T = 0; T < noPoints; T++) {
    for (i = 0; i < noObjectives; i++) {
      if (fscanf(file, "%lf", &value) != EOF) {
	    (*frontPtr)[T][i] = value;
      }
      else {
        return 0;
      }
    }
    if (i > 0 && i < noObjectives)  ERROR("data in file incomplete");
  }
  return noPoints;
} /* ReadFront */

void  DeallocateFront(double**  front, int  noPoints) {
  int  i;

  if (front != NULL) {
    for (i = 0; i < noPoints; i++)
      if (front[i] != NULL)
	free(front[i]);
    free(front);
  }
} /* DeallocateFront */


