using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using business;
using Lib.DataTypes;
using Lib.Entities;

namespace presentation {
	public partial class AddGuestRequestsPage : ValidatedPage {
		private MainWindow MainWindow { get; }

		private IBusiness Business { get; }

		private Session<Guest> GuestSession { get; }

		private Guest Guest {
			get => GuestSession.Person;
		}

		private int NumberOfAdults;

		private int NumberOfChildren;

		private IEnumerable<City> Region {
			get; // TODO: Make this return an enumerable of the checked cities
		}

		private IEnumerable<Amenity> DesiredAmenities {
			get; // TODO: Make this return an enumerable of the checked cities
		}

		private IEnumerable<Unit.Type> DesiredUnitTypes {
			get; // TODO: Make this return an enumerable of the checked cities
		}

		public AddGuestRequestsPage(IBusiness business, Session<Guest> guest_session) {
			InitializeComponent();
			Business = business;
			GuestSession = guest_session;

			Validators = new List<IValidator>() {
				new Validator<DatePicker>(start_date, start_date_error,
						date_picker => date_picker.SelectedDate == null ? "Error: Start date is required." : ""
					),

					new Validator<DatePicker>(end_date, end_date_error,
						date_picker => date_picker.SelectedDate == null ? "Error: End date is required." : ""
					),

					new Validator<TextBox>(number_of_adults, number_of_adults_error,
						control => control.Text == "" ? "Error: Number of adults is required." : "",
						control => int.TryParse(control.Text, out NumberOfAdults) ? "" : "Error: Could not interpret the input as a number."
					),

					new Validator<TextBox>(number_of_children, number_of_children_error,
						control => control.Text == "" ? "Error: Number of children is required." : "",
						control => int.TryParse(control.Text, out NumberOfChildren) ? "" : "Error: Could not interpret the input as a number."
					)
			};

		}

		private void AddGuestRequest(object sender, RoutedEventArgs e) {
			if (!Validate()) {
				return;
			}

			try {
				Business.AddGuestRequest(new GuestRequest(
					Guest,
					((DateTime) start_date.SelectedDate).ToDate(),
					((DateTime) end_date.SelectedDate).ToDate(),
					NumberOfAdults,
					NumberOfChildren,
					Region.ToHashSet(),
					DesiredUnitTypes.ToHashSet(),
					DesiredAmenities.ToHashSet()
				));
			}
			// TODO: Catch all the errors that the try might throw instead of catching Exception
			catch (Exception) {
				return;
			}
		}

		private void Cancel(object sender, RoutedEventArgs e) {
			GuestSession.SignOut();
		}
	}
}