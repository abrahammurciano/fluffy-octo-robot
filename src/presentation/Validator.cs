using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace presentation {
	internal class Validator<TControl> where TControl : Control {
		private TControl Control { get; }

		private TextBlock ErrorBlock { get; }

		private Brush InitialBorder { get; }

		private ICollection<Func<TControl, string>> Checks { get; } = new List<Func<TControl, string>>();

		public Validator(TControl control, TextBlock error_block, params Func<TControl, string>[] checks) {
			Control = control;
			ErrorBlock = error_block;
			InitialBorder = control.BorderBrush;
			foreach (Func<TControl, string> check in checks) {
				AddCheck(check);
			}
		}

		public bool Validate() {
			foreach (Func<TControl, string> check in Checks) {
				string result = check(Control);
				if (result != "") {
					SetError(result);
					return false;
				}
			}
			ResetError();
			return true;
		}

		public void AddCheck(Func<TControl, string> check) {
			Checks.Add(check);
		}

		public void SetError(string message) {
			Control.BorderBrush = Brushes.Red;
			ErrorBlock.Text = message;
		}

		public void ResetError() {
			Control.BorderBrush = InitialBorder;
			ErrorBlock.Text = "";
		}
	}
}