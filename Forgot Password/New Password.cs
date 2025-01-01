﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sims.Forgot_Password
{
    public partial class New_Password : Form
    {
        private Forgot_Password username;
        public string Username { get; set; }

        public New_Password(Forgot_Password username)
        {
            InitializeComponent();
            this.username = username;
            InitializePasswordTextBox();
            ConfirmPasswordTextBox();
            showNewPasswordChk.OnChange += new EventHandler(showNewPasswordChk_OnChange);
            confirmPasswordCHk.OnChange += new EventHandler(showNewPasswordChk_OnChange);
        }

        private void InitializePasswordTextBox()
        {
            newPasswordTxt.PlaceholderText = "Create new password";

            newPasswordTxt.PasswordChar = '\0';

            newPasswordTxt.TextChanged += (sender, e) =>
            {
                string enteredPassword = newPasswordTxt.Text;
                if (string.IsNullOrEmpty(enteredPassword))
                {
                    newPasswordTxt.PlaceholderText = "Create new password";
                    newPasswordTxt.PasswordChar = '\0';
                    newPasswordTxt.BorderColorActive = Color.Gray;
                    passwordStrengthLabel.Text = "";
                }
                else
                {
                    newPasswordTxt.PlaceholderText = "";
                    newPasswordTxt.PasswordChar = '●';
                    if (enteredPassword.Length < 8)
                    {
                        newPasswordTxt.FillColor = Color.Red;
                        passwordStrengthLabel.Text = "Password too short, must be at least 8 characters.";
                    }
                    else if (!enteredPassword.Any(char.IsDigit) || !enteredPassword.Any(char.IsLetter))
                    {
                        newPasswordTxt.FillColor = Color.Orange;
                        passwordStrengthLabel.Text = "Password must contain both letters and numbers.";
                    }
                    else if (!enteredPassword.Any(char.IsUpper))
                    {
                        newPasswordTxt.FillColor = Color.Orange;
                        passwordStrengthLabel.Text = "Password must contain at least one uppercase letter.";
                    }
                    else
                    {
                        newPasswordTxt.FillColor = Color.Green;
                        passwordStrengthLabel.Text = "Password strength is good.";
                    }
                }

            };
        }

        private void ConfirmPasswordTextBox()
        {
            confirmPasswordTxt.PlaceholderText = "Confirm your password";
            confirmPasswordTxt.PasswordChar = '\0';
            confirmPasswordTxt.TextChanged += (sender, e) =>
            {
                string enteredPassword = confirmPasswordTxt.Text;
                if (string.IsNullOrEmpty(enteredPassword))
                {
                    confirmPasswordTxt.PlaceholderText = "Confirm your password";
                    confirmPasswordTxt.PasswordChar = '\0';
                }
                else
                {
                    confirmPasswordTxt.PlaceholderText = "";
                    confirmPasswordTxt.PasswordChar = '●';
                }
            };
        }

        private void ContinueBtn_Click(object sender, EventArgs e)
        {
            Username = username.UsernameTxt.Text;
            string newPassword = newPasswordTxt.Text;
            string confirmPassword = confirmPasswordTxt.Text;

            if (ChangePasswordInDatabase(newPassword, confirmPassword))
            {
                MessageBox.Show("Password changed successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                new Login_Form().Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("New password and confirm password do not match", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool IsPasswordConfirmed(string password, string confirmPassword)
        {
            return password == confirmPassword;
        }

        private bool ChangePasswordInDatabase(string newPassword, string confirmPassword)
        {
            dbModule db = new dbModule();
            if (!IsPasswordConfirmed(newPassword, confirmPassword))
            {
                return false;
            }

            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE users SET password = @password WHERE username = @username";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Password", newPassword); // Use the newPassword parameter passed to the function.
                        cmd.Parameters.AddWithValue("@username", Username); // Use the Username property to identify the user.

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that may occur during the update process.
                    MessageBox.Show("Error updating password: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        private void showNewPasswordChk_OnChange(object sender, EventArgs e)
        {
            if (showNewPasswordChk.Checked)
            {
                newPasswordTxt.PasswordChar = '\0';
            }
            else
            {
                newPasswordTxt.PasswordChar = '●';
            }
        }

        private void confirmPasswordCHk_OnChange(object sender, EventArgs e)
        {
            if (confirmPasswordCHk.Checked)
            {
                confirmPasswordTxt.PasswordChar = '\0';
            }
            else
            {
                confirmPasswordTxt.PasswordChar = '●';
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            new Forgot_Password().Show();
            this.Hide();
        }
    }
}
