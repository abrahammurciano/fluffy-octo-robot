using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using business;
using Lib.DataTypes;
using Lib.Entities;
using Lib.Exceptions;

namespace presentation {
	public partial class AddHostPage : ValidatedPage {
		private IBusiness Business { get; }

		private Frame Frame { get; }

		private HostSignInPage HostSignInPage { get; }

		private BankBranch BankBranch { get; set; }

		private Validator<TextBox> EmailValidator { get; }

		public AddHostPage(IBusiness business, Frame frame, HostSignInPage host_sign_in_page) {
			InitializeComponent();
			Business = business;
			Frame = frame;
			HostSignInPage = host_sign_in_page;

			EmailValidator = new Validator<TextBox>(email, email_error,
				control => control.Text == "" ? "Error: Email is required." : "",
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
				new Validator<TextBox>(first_name, first_name_error,
						control => control.Text == "" ? "Error: First name is required." : "",
						control => Regex.Match(control.Text, @"^[a-z ,.'-]+$", RegexOptions.IgnoreCase).Success ? "" : "Error: Cannot have these symbols in your name."
					),

					new Validator<TextBox>(last_name, last_name_error,
						control => control.Text == "" ? "Error: First name is required." : "",
						control => Regex.Match(control.Text, @"^[a-z ,.'-]+$", RegexOptions.IgnoreCase).Success ? "" : "Error: Cannot have these symbols in your name."
					),

					EmailValidator,

					new Validator<TextBox>(phone, phone_error,
						control => control.Text == "" ? "Error: Phone number is required." : "",
						control => {
							try {
								control.Text = new Phone(control.Text);
								return "";
							} catch (InvalidPhoneException error) {
								return error.Message;
							}
						}
					),

					new Validator<PasswordBox>(password, password_error,
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

					new Validator<PasswordBox>(repeat_password, repeat_password_error,
						control => control.Password != password.Password ? "Error: Passwords do not match." : ""
					),

					new Validator<TextBox>(bank_number, bank_number_error,
						control => {
							try {
								control.Text = new ID(control.Text, 2);
								Business.BankBranches().First(branch => branch.BankID == bank_number.Text);
								return "";
							} catch (IncorrectDigitsException) {
								return "Error: Bank number must be at most two digits.";
							} catch (FormatException) {
								return "Error: Could not parse the input as a bank number.";
							} catch (InvalidOperationException) {
								return "Error: No bank with this number was found.";
							}
						}
					),

					new Validator<TextBox>(branch_number, branch_number_error,
						control => {
							try {
								control.Text = new ID(control.Text, 3);
								BankBranch = Business.BankBranches().First(branch => branch.BankID == bank_number.Text && branch.BranchID == control.Text);
								return "";
							} catch (IncorrectDigitsException) {
								return "Error: Branch number must be at most three digits.";
							} catch (FormatException) {
								return "Error: Could not parse the input as a branch number.";
							} catch (InvalidOperationException) {
								return "Error: No branch with this number was found for bank " + bank_number.Text + '.';
							}
						}
					),

					new Validator<TextBox>(
						account_number,
						account_number_error,
						control => {
							try {
								control.Text = new ID(control.Text, 6);
								return "";
							} catch (IncorrectDigitsException) {
								return "Error: Account number must be six digits.";
							} catch (FormatException) {
								return "Error: Could not parse the input as an account number.";
							}
						}
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
				Business.AddHost(new Host(first_name.Text, last_name.Text, email.Text, phone.Text, password.Password, BankBranch, account_number.Text));
				EmailValidator.ResetError();

				HostSignInPage.email.Text = email.Text;
				HostSignInPage.password.Password = password.Password;
				Frame.GoBack();
				HostSignInPage.SignIn();
			} catch (EmailExistsException) {
				EmailValidator.SetError("Error: A host already exists with this email. Try signing in instead.");
			}
		}

		private void Cancel(object sender, RoutedEventArgs e) {
			Frame.GoBack();
		}
	}
}