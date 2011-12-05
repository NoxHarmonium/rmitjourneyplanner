// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Register.aspx.cs" company="">
//   
// </copyright>
// <summary>
//   The register.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebInterface.Account
{
    using System;
    using System.Web.Security;

    /// <summary>
    /// The register.
    /// </summary>
    public partial class Register : System.Web.UI.Page
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
            this.RegisterUser.ContinueDestinationPageUrl = this.Request.QueryString["ReturnUrl"];
        }

        /// <summary>
        /// The register user_ created user.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void RegisterUser_CreatedUser(object sender, EventArgs e)
        {
            FormsAuthentication.SetAuthCookie(this.RegisterUser.UserName, false /* createPersistentCookie */);

            string continueUrl = this.RegisterUser.ContinueDestinationPageUrl;
            if (string.IsNullOrEmpty(continueUrl))
            {
                continueUrl = "~/";
            }

            this.Response.Redirect(continueUrl);
        }

        #endregion
    }
}