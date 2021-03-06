using System.Windows;
using System.Windows.Controls;
using business;
using Lib.DataTypes;
using Lib.Entities;
using Lib.Exceptions;

namespace presentation {
	public partial class HostSignInPage : Page {
		private IBusiness Business { get; }

		private Session<Host> HostSession { get; }

		private Frame Frame { get; }

		private IValidator EmailValidator { get; }

		private IValidator PasswordValidator { get; }

		public HostSignInPage(IBusiness business, Session<Host> host_session, Frame frame) {
			InitializeComponent();
			Business = business;
			host_session.SignInPage = this;
			HostSession = host_session;
			Frame = frame;

			EmailValidator = new EmailValidator(email, email_error);

			PasswordValidator = new PasswordValidator(password, password_error);
		}

		public void SignIn() {
			if (!EmailValidator.Validate() || !PasswordValidator.Validate()) {
				return;
			}

			try {
				Host host = Business.Host(new Email(email.Text));
				EmailValidator.ResetError();

				HostSession.SignIn(host, password.Password);
				PasswordValidator.ResetError();

				Frame.Navigate(new HostPage(Business, HostSession, Frame));
			} catch (InexistentEmailException ex) {
				EmailValidator.SetError(ex.Message);
			} catch (WrongPasswordException ex) {
				PasswordValidator.SetError(ex.Message);
			}
		}

		private void SignIn(object sender, RoutedEventArgs e) {
			SignIn();
		}

		private void SignUp(object sender, RoutedEventArgs e) {
			Frame.Navigate(new AddHostPage(Business, Frame, this));
		}
	}
}