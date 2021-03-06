using System;
using System.Collections.Generic;
using System.Text;
using Lib.DataTypes;
using Lib.Interfaces;

namespace Lib.Entities {
	public abstract class Person : User {
		public virtual string FirstName { get; }

		public virtual string LastName { get; }

		public virtual string Name {
			get => FirstName + ' ' + LastName;
		}
		public virtual Phone Phone { get; }

		public Person(ID id, string first_name, string last_name, Email email, Phone phone, IEnumerable<byte> password_hash) : base(id, email, password_hash) {
			FirstName = first_name;
			LastName = last_name;
			Phone = phone;
		}

		public override string ToString() {
			return ID.ToString() + " - " + Name;
		}

		public override bool Equals(object obj) {
			return obj is Person person
				&& (person.GetType().IsAssignableFrom(GetType()) || GetType().IsAssignableFrom(person.GetType()))
				&& EqualityComparer<ID>.Default.Equals(ID, person.ID);
		}

		public override int GetHashCode() {
			int hashCode = -745477792;
			hashCode = hashCode * -1521134295 + EqualityComparer<ID>.Default.GetHashCode(ID);
			hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(GetType());
			return hashCode;
		}
	}
}