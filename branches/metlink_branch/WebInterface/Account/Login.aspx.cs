// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Login.aspx.cs" company="">
//   
// </copyright>
// <summary>
//   The login.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebInterface.Account
{
    using System;
    using System.Web;

    /// <summary>
    /// The login.
    /// </summary>
    public partial class Login : System.Web.UI.Page
    {
        #region Methods

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl="
                                                 + HttpUtility.UrlEncode(this.Request.QueryString["ReturnUrl"]);
        }

        #endregion
    }
}