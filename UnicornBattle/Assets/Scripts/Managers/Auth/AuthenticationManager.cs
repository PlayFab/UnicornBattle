using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornBattle.Managers.Auth
{
    public abstract class AuthenticationManager
    {
        #region abstract methods
        public abstract bool IsLoggedIn();
        public abstract bool IsLoggedOut();
        public abstract string GetDeviceId();
        public abstract void LoginWithDeviceId(bool p_shouldCreateNewAccount, System.Action<string> p_onSuccessCallback, System.Action<string> p_onFailureCallback);
        public abstract void LoginWithUsername(string p_user, string p_password, System.Action<string> p_onSuccessCallback, System.Action<string> p_onFailureCallback);
        public abstract void LoginWithEmail(string p_user, string p_password, System.Action<string> p_onSuccessCallback, System.Action<string> p_onFailureCallback);
        public abstract void Logout(System.Action<string> p_onSuccessCallback, System.Action<string> p_onFailureCallback);
        public abstract void RegisterNewAccount(
            string p_username,
            string p_password,
            string p_email,
            System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback);

        public abstract void SendAccountRecoveryEmail(string email, System.Action<string> p_onSuccessCallback, System.Action<string> p_onFailureCallback);
        public abstract void LinkDeviceId();
        public abstract void UnlinkDeviceId();

        #endregion

        #region utilities

        /// <summary>
        /// email pattern for validating emails
        /// </summary>
        protected const string emailPattern = @"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$";

        /// <summary>
        /// Validates the email.
        /// </summary>
        /// <returns><c>true</c>, if email was validated, <c>false</c> otherwise.</returns>
        /// <param name="em">Email address</param>
        public static bool ValidateEmail(string em)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(em, emailPattern);
        }

        /// <summary>
        /// Validates the password.
        /// </summary>
        /// <returns><c>true</c>, if password was validated, <c>false</c> otherwise.</returns>
        /// <param name="p1">P1, text from password field one</param>
        /// <param name="p2">P2, text from password field two</param>
        public static bool ValidatePassword(string p1, string p2)
        {
            return ((p1 == p2) && p1.Length > 5);
        }

        #endregion
    }
}
