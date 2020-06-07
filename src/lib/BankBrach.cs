using System.Text;
using exceptions;

namespace lib {
	public class BankBranch {

		public string BankName {
			get;
			private set;
		}

		public string BranchAddress {
			get;
			private set;
		}

		public City BranchCity {
			get;
			private set;
		}

		private ID bank_id;
		public ID BankID {
			get => bank_id;
			private set {
				if (value.Digits != 3) {
					throw new IncorrectDigitsException(value.Number, value.Digits, "Error: Bank ID must be three digits.");
				}
				bank_id = value;
			}
		}

		private ID branch_id;
		public ID BranchID {
			get => branch_id;
			private set {
				if (value.Digits != 3) {
					throw new IncorrectDigitsException(value.Number, value.Digits, "Error: Branch ID must be three digits.");
				}
				branch_id = value;
			}
		}

		public BankBranch(ID bank_id, string bank_name, ID branch_id, City branch_city) {
			this.BankID = bank_id;
			BankName = bank_name;
			this.BranchID = branch_id;
			BranchCity = branch_city;
		}

		public string ToString(int tabs) {
			StringBuilder sb = new StringBuilder();
			sb.Append('\t', tabs);
			sb.Append("Bank Details:");
			sb.Append("\n");

			sb.Append('\t', tabs);
			sb.Append("-------------");
			sb.Append("\n");

			sb.Append('\t', tabs);
			sb.Append("Bank Name:\t");
			sb.Append(BankName);
			sb.Append("\n");

			sb.Append('\t', tabs);
			sb.Append("Bank Number:\t");
			sb.Append(BankID);
			sb.Append("\n");

			sb.Append('\t', tabs);
			sb.Append("Branch Number:\t");
			sb.Append(BranchID);
			sb.Append("\n");

			sb.Append('\t', tabs);
			sb.Append("Branch Address:\t");
			sb.Append(BranchAddress);
			sb.Append("\n");

			sb.Append('\t', tabs);
			sb.Append("Branch City:\t");
			sb.Append(BranchCity);
			sb.Append("\n");

			return sb.ToString();
		}
	}
}