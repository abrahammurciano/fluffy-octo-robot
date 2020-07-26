using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using business;
using Lib.DataTypes;
using Lib.Entities;
using Lib.Exceptions;

namespace presentation {
	public partial class AddGuestPage : ValidatedPage {
		private IBusiness Business { get; }

		private Frame Frame { get; }

		private GuestSignInPage GuestSignInPage { get; }

		private BankBranch BankBranch { get; set; }

		private RequiredTextValidator EmailValidator { get; }

		public AddGuestPage(IBusiness business, Frame frame, GuestSignInPage Guest_sign_in_page) {
			InitializeComponent();
			Business = business;
			Frame = frame;
			GuestSignInPage = Guest_sign_in_page;

			EmailValidator = new RequiredTextValidator(
				email,
				email_error,
				control => {
					try {
						control.Text = new Email(control.Text);
						return "";
					} catch (InvalidEmailException error) {
						return error.Message;
					}
				}
			);

			Validators = new List<IValidator>() {
				new RequiredTextValidator(
						first_name,
						first_name_error,
						control => Regex.Match(control.Text, @"^[a-z ,.'-]+$", RegexOptions.IgnoreCase).Success ? "" : "Error: Cannot have these symbols in your name."
					),

					new RequiredTextValidator(
						last_name,
						last_name_error,
						control => Regex.Match(control.Text, @"^[a-z ,.'-]+$", RegexOptions.IgnoreCase).Success ? "" : "Error: Cannot have these symbols in your name."
					),

					EmailValidator,

					new RequiredTextValidator(
						phone,
						phone_error,
						control => {
							try {
								control.Text = new Phone(control.Text);
								return "";
							} catch (InvalidPhoneException error) {
								return error.Message;
							}
						}
					),

					new Validator<PasswordBox>(
						password,
						password_error,
						control => control.Password == "" ? "Error: Password is required." : "",
						control => {
							try {
								control.Password = new Password(control.Password);
								return "";
							} catch (InvalidPasswordException error) {
								return error.Message;
							}
						}
					),

					new Validator<PasswordBox>(
						repeat_password,
						repeat_password_error,
						control => control.Password != password.Password ? "Error: Passwords do not match." : ""
					)
			};
		}

		private void SignUp(object sender, RoutedEventArgs e) {
			first_name.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(first_name.Text.ToLower());
			last_name.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(last_name.Text.ToLower());

			if (!Validate()) {
				return;
			}

			try {
				Business.AddGuest(new Guest(first_name.Text, last_name.Text, email.Text, phone.Text, password.Password));
				EmailValidator.ResetError();

				GuestSignInPage.email.Text = email.Text;
				GuestSignInPage.password.Password = password.Password;
				Frame.GoBack();
				GuestSignInPage.SignIn();
			} catch (EmailExistsException) {
				EmailValidator.SetError("Error: A Guest already exists with this email. Try signing in instead.");
			}
		}

		private void Cancel(object sender, RoutedEventArgs e) {
			Frame.GoBack();
		}
	}
}