using System.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.Type;
using NUnit.Framework;
using SharpTestsEx;
using System;

namespace NHibernate.Test.TypesTest
{
	public class DateTypeTest
	{
		[Test]
		public void WhenNoParameterThenDefaultValueIsBaseDateValue()
		{
			var dateType = new DateType();
			dateType.DefaultValue.Should().Be(DateType.BaseDateValue);
		}

		[Test]
		public void WhenSetParameterThenDefaultValueIsParameterValue()
		{
			var dateType = new DateType();
			dateType.SetParameterValues(new Dictionary<string, string>{{DateType.BaseValueParameterName, "0001/01/01"}});
			dateType.DefaultValue.Should().Be(DateTime.MinValue);
		}

		[Test]
		public void WhenSetParameterNullThenNotThrow()
		{
			var dateType = new DateType();
			dateType.Executing(dt=> dt.SetParameterValues(null)).NotThrows();
		}
	}

	public class DateTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "Date"; }
		}

		[Test]
		public void ShouldBeDateType()
		{
			if (!(Dialect is MsSql2008Dialect))
			{
				Assert.Ignore("This test does not apply to " + Dialect);
			}
			var sqlType = Dialect.GetTypeName(NHibernateUtil.Date.SqlType);
			sqlType.ToLowerInvariant().Should().Be("date");
		}

		[Test]
		public void ReadWriteNormal()
		{
			var expected = DateTime.Today.Date;

			var basic = new DateClass { DateValue = expected.AddHours(1) };
			object savedId;
			using (ISession s = OpenSession())
			{
				savedId = s.Save(basic);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				basic = s.Get<DateClass>(savedId);
				basic.DateValue.Should().Be(expected);
				s.Delete(basic);
				s.Flush();
			}
		}

		[Test]
		public void ReadWriteBaseValue()
		{
			var basic = new DateClass { DateValue = new DateTime(1899,1,1) };
			object savedId;
			using (ISession s = OpenSession())
			{
				savedId = s.Save(basic);
				s.Flush();
			}
			using (ISession s = OpenSession())
			{
				basic = s.Get<DateClass>(savedId);
				basic.DateValue.HasValue.Should().Be.False();
				s.Delete(basic);
				s.Flush();
			}
		}
	}
}