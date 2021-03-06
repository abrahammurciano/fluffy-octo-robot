using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using business;
using Lib.Entities;
using Lib.Exceptions;

namespace presentation {
	public partial class GuestPage : Page {
		private IBusiness Business { get; }

		private Frame Frame { get; }

		private Session<Guest> GuestSession { get; }

		public Guest Guest {
			get => GuestSession.User;
		}

		public ObservableCollection<GuestRequest> GuestRequests { get; }

		public GuestPage(IBusiness business, Session<Guest> guest_session, Frame frame) {
			InitializeComponent();
			Business = business;
			GuestSession = guest_session;
			Frame = frame;
			GuestRequests = new ObservableCollection<GuestRequest>(Business.GuestRequests());
			DataContext = this;
		}

		private void SignOut(object sender, RoutedEventArgs e) {
			GuestSession.SignOut();
		}

		private void NewGuestRequest(object sender, RoutedEventArgs e) {
			Frame.Navigate(new AddGuestRequestsPage(Business, Frame, Guest, GuestRequests, GuestSession));
		}

		private void IgnorePreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			HandlePreviewMouseWheel.IgnorePreviewMouseWheel(sender, e);
		}

		private void ViewGuestRequest(object sender, RoutedEventArgs e) {
			GuestRequest guest_request = (sender as Button).CommandParameter as GuestRequest;
			Frame.Navigate(new ViewGuestRequestPage(Business, Frame, guest_request));
		}

		private void EditGuestRequest(object sender, RoutedEventArgs e) {
			GuestRequest guest_request = (sender as Button).CommandParameter as GuestRequest;
			Frame.Navigate(new EditGuestRequestPage(Business, Frame, guest_request, GuestRequests));
		}

		private async void DeleteGuestRequest(object sender, RoutedEventArgs e) {
			GuestRequest guest_request = (sender as Button).CommandParameter as GuestRequest;
			if ((bool) await MaterialDesignThemes.Wpf.DialogHost.Show(guest_request, "confirm_request_delete")) {
				try {
					Business.DeleteGuestRequest(guest_request);
					GuestRequests.Remove(guest_request);
				} catch (DeletingGuestRequestWithConfirmedOrderException ex) {
					await MaterialDesignThemes.Wpf.DialogHost.Show(ex, "request_delete_error");
				}
			}
		}
	}
}