using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Lib.DataTypes;
using Lib.Entities;

namespace data {
	class UnitXmlConverter : IXmlConverter<Unit> {
		private DataAccessorReadOnly<ID, Host> Hosts {
			get => DataFactory.Data.Host;
		}

		public Unit XmlToObj(XElement element) {
			ICollection<Amenity> amenities = new HashSet<Amenity>();
			foreach (XElement amenity in element.Element("amenities").Elements()) {
				amenities.Add(amenity.Value.Trim());
			}

			Unit.Calendar calendar = new Unit.Calendar();
			foreach (XElement booking_xml in element.Element("calendar").Elements()) {
				calendar.Bookings.Add(
					new Unit.Calendar.Booking(
						Date.Parse(booking_xml.Element("start").Value.Trim()),
						int.Parse(booking_xml.Element("duration").Value.Trim())
					)
				);
			}

			return new Unit(
				element.Element("id").Value.Trim(),
				Hosts[element.Element("host_id").Value.Trim()],
				element.Element("name").Value.Trim(),
				element.Element("city").Value.Trim(),
				amenities,
				element.Element("unit_type").Value.Trim(),
				calendar
			);
		}

		public XElement ObjToXml(Unit unit) {
			XElement calendar_xml = new XElement("calendar");
			foreach (Unit.Calendar.Booking booking in unit.Bookings) {
				calendar_xml.Add(
					new XElement(
						"booking",
						new XElement("start", booking.Start.ToString("yyyy-MM-dd")),
						new XElement("duration", booking.Duration)
					)
				);
			}

			XElement amenities_xml = new XElement("amenities");
			foreach (Amenity amenity in unit.Amenities) {
				amenities_xml.Add(
					new XElement("amenity", amenity)
				);
			}

			return new XElement(
				"unit",
				new XElement("id", unit.ID),
				calendar_xml,
				new XElement("host_id", unit.Host.ID),
				new XElement("name", unit.UnitName),
				new XElement("city", unit.City),
				amenities_xml,
				new XElement("unit_type", unit.UnitType)
			);
		}
	}
}