﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Linq;
using NHibernate.Transform;
using NUnit.Framework;
using NHibernate.Criterion;

namespace NHibernate.Test.SqlTest.Query
{
	using System.Threading.Tasks;
	[TestFixture]
	public class GeneralTestAsync : TestCase
	{
		protected const string OrganizationFetchJoinEmploymentSQL =
			"SELECT org.ORGID as {org.id}, " +
			"        org.NAME as {org.name}, " +
			"        emp.EMPLOYER as {emp.key}, " +
			"        emp.EMPID as {emp.element}, " +
			"        {emp.element.*}  " +
			"FROM ORGANIZATION org " +
			"    LEFT OUTER JOIN EMPLOYMENT emp ON org.ORGID = emp.EMPLOYER";

		protected const string OrganizationJoinEmploymentSQL =
			"SELECT org.ORGID as {org.id}, " +
			"        org.NAME as {org.name}, " +
			"        {emp.*}  " +
			"FROM ORGANIZATION org " +
			"    LEFT OUTER JOIN EMPLOYMENT emp ON org.ORGID = emp.EMPLOYER";

		protected const string EmploymentSQL = "SELECT * FROM EMPLOYMENT";

		protected string EmploymentSQLMixedScalarEntity =
			"SELECT e.*, e.employer as employerid  FROM EMPLOYMENT e";

		protected const string OrgEmpRegionSQL =
			"select {org.*}, {emp.*}, emp.REGIONCODE " +
			"from ORGANIZATION org " +
			"     left outer join EMPLOYMENT emp on org.ORGID = emp.EMPLOYER";

		protected string OrgEmpPersonSQL =
			"select {org.*}, {emp.*}, {pers.*} " +
			"from ORGANIZATION org " +
			"    join EMPLOYMENT emp on org.ORGID = emp.EMPLOYER " +
			"    join PERSON pers on pers.PERID = emp.EMPLOYEE ";

		protected override string[] Mappings
		{
			get { return new[] {"SqlTest.Query.NativeSQLQueries.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public async Task FailOnNoAddEntityOrScalarAsync()
		{
			// Note: this passes, but for the wrong reason.
			//      there is actually an exception thrown, but it is the database
			//      throwing a sql exception because the SQL gets passed
			//      "un-processed"...
			ISession s = OpenSession();
			try
			{
				string sql = "select {org.*} " +
				             "from organization org";
				await (s.CreateSQLQuery(sql).ListAsync());
				Assert.Fail("Should throw an exception since no AddEntity nor AddScalar has been performed.");
			}
			catch (HibernateException)
			{
				// expected behavior
			}
			finally
			{
				s.Close();
			}
		}

		[Test]
		public async Task SQLQueryInterfaceAsync()
		{
			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");
			Person gavin = new Person("Gavin");
			Employment emp = new Employment(gavin, jboss, "AU");

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				await (s.SaveAsync(ifa));
				await (s.SaveAsync(jboss));
				await (s.SaveAsync(gavin));
				await (s.SaveAsync(emp));

				IList l = await (s.CreateSQLQuery(OrgEmpRegionSQL)
							.AddEntity("org", typeof(Organization))
							.AddJoin("emp", "org.employments")
							.AddScalar("regionCode", NHibernateUtil.String)
							.ListAsync());
				Assert.AreEqual(2, l.Count);

				l = await (s.CreateSQLQuery(OrgEmpPersonSQL)
					.AddEntity("org", typeof(Organization))
					.AddJoin("emp", "org.employments")
					.AddJoin("pers", "emp.employee")
					.ListAsync());
				Assert.AreEqual(l.Count, 1);

				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var l = await (s.CreateSQLQuery(
							"select {org.*}, {emp.*} " +
							"from ORGANIZATION org " +
							"     left outer join EMPLOYMENT emp on org.ORGID = emp.EMPLOYER, ORGANIZATION org2")
						.AddEntity("org", typeof(Organization))
						.AddJoin("emp", "org.employments")
						.SetResultTransformer(new DistinctRootEntityResultTransformer())
						.ListAsync());
				Assert.AreEqual(l.Count, 2);

				await (t.CommitAsync());
				s.Close();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.DeleteAsync(emp));
				await (s.DeleteAsync(gavin));
				await (s.DeleteAsync(ifa));
				await (s.DeleteAsync(jboss));

				await (t.CommitAsync());
				s.Close();
			}
		}

		[Test]
		public async Task SQLQueryInterfaceCacheableAsync()
		{
			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");
			Person gavin = new Person("Gavin");
			Employment emp = new Employment(gavin, jboss, "AU");

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				await (s.SaveAsync(ifa));
				await (s.SaveAsync(jboss));
				await (s.SaveAsync(gavin));
				await (s.SaveAsync(emp));

				IList l = await (s.CreateSQLQuery(OrgEmpRegionSQL)
							.AddEntity("org", typeof(Organization))
							.AddJoin("emp", "org.employments")
							.AddScalar("regionCode", NHibernateUtil.String)
							.SetCacheable(true)
							.ListAsync());
				Assert.AreEqual(2, l.Count);

				l = await (s.CreateSQLQuery(OrgEmpPersonSQL)
					.AddEntity("org", typeof(Organization))
					.AddJoin("emp", "org.employments")
					.AddJoin("pers", "emp.employee")
					.SetCacheable(true)
					.ListAsync());
				Assert.AreEqual(l.Count, 1);

				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var l = await (s.CreateSQLQuery(
							"select {org.*}, {emp.*} " +
							"from ORGANIZATION org " +
							"     left outer join EMPLOYMENT emp on org.ORGID = emp.EMPLOYER, ORGANIZATION org2")
						.AddEntity("org", typeof(Organization))
						.AddJoin("emp", "org.employments")
						.SetCacheable(true)
						.SetResultTransformer(new DistinctRootEntityResultTransformer())
						.ListAsync());
				Assert.AreEqual(l.Count, 2);

				await (t.CommitAsync());
				s.Close();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.DeleteAsync(emp));
				await (s.DeleteAsync(gavin));
				await (s.DeleteAsync(ifa));
				await (s.DeleteAsync(jboss));

				await (t.CommitAsync());
				s.Close();
			}
		}

		[Test]
		public async Task ResultSetMappingDefinitionAsync()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");
			Person gavin = new Person("Gavin");
			Employment emp = new Employment(gavin, jboss, "AU");

			await (s.SaveAsync(ifa));
			await (s.SaveAsync(jboss));
			await (s.SaveAsync(gavin));
			await (s.SaveAsync(emp));

			IList l = await (s.CreateSQLQuery(OrgEmpRegionSQL)
			           .SetResultSetMapping("org-emp-regionCode")
			           .ListAsync());
			Assert.AreEqual(l.Count, 2);

			l = await (s.CreateSQLQuery(OrgEmpPersonSQL)
			     .SetResultSetMapping("org-emp-person")
			     .ListAsync());
			Assert.AreEqual(l.Count, 1);

			await (s.DeleteAsync(emp));
			await (s.DeleteAsync(gavin));
			await (s.DeleteAsync(ifa));
			await (s.DeleteAsync(jboss));

			await (t.CommitAsync());
			s.Close();
		}

		[Test]
		public async Task ScalarValuesAsync()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");

			object idIfa = await (s.SaveAsync(ifa));
			object idJBoss = await (s.SaveAsync(jboss));

			await (s.FlushAsync());

			IList result = await (s.GetNamedQuery("orgNamesOnly").ListAsync());
			Assert.IsTrue(result.Contains("IFA"));
			Assert.IsTrue(result.Contains("JBoss"));

			result = await (s.GetNamedQuery("orgNamesOnly").SetResultTransformer(CriteriaSpecification.AliasToEntityMap).ListAsync());
			IDictionary m = (IDictionary) result[0];
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(1, m.Count);
			Assert.IsTrue(m.Contains("NAME"));

			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			IEnumerator iter = (await (s.GetNamedQuery("orgNamesAndOrgs").ListAsync())).GetEnumerator();
			iter.MoveNext();
			object[] o = (object[]) iter.Current;
			Assert.AreEqual(o[0], "IFA");
			Assert.AreEqual(((Organization) o[1]).Name, "IFA");
			iter.MoveNext();
			o = (object[]) iter.Current;
			Assert.AreEqual(o[0], "JBoss");
			Assert.AreEqual(((Organization) o[1]).Name, "JBoss");

			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			// test that the ordering of the results is truly based on the order in which they were defined
			iter = (await (s.GetNamedQuery("orgsAndOrgNames").ListAsync())).GetEnumerator();
			iter.MoveNext();
			object[] row = (object[]) iter.Current;
			Assert.AreEqual(typeof(Organization), row[0].GetType(), "expecting non-scalar result first");
			Assert.AreEqual(typeof(string), row[1].GetType(), "expecting scalar result second");
			Assert.AreEqual("IFA", ((Organization) row[0]).Name);
			Assert.AreEqual(row[1], "IFA");
			iter.MoveNext();
			row = (object[]) iter.Current;
			Assert.AreEqual(typeof(Organization), row[0].GetType(), "expecting non-scalar result first");
			Assert.AreEqual(typeof(string), row[1].GetType(), "expecting scalar result second");
			Assert.AreEqual(((Organization) row[0]).Name, "JBoss");
			Assert.AreEqual(row[1], "JBoss");
			Assert.IsFalse(iter.MoveNext());

			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();

			iter = (await (s.GetNamedQuery("orgIdsAndOrgNames").ListAsync())).GetEnumerator();
			iter.MoveNext();
			o = (object[]) iter.Current;
			Assert.AreEqual(o[1], "IFA");
			Assert.AreEqual(o[0], idIfa);
			iter.MoveNext();
			o = (object[]) iter.Current;
			Assert.AreEqual(o[1], "JBoss");
			Assert.AreEqual(o[0], idJBoss);

			await (s.DeleteAsync(ifa));
			await (s.DeleteAsync(jboss));
			await (t.CommitAsync());
			s.Close();
		}

		[Test]
		public async Task MappedAliasStrategyAsync()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");
			Person gavin = new Person("Gavin");
			Employment emp = new Employment(gavin, jboss, "AU");
			await (s.SaveAsync(jboss));
			await (s.SaveAsync(ifa));
			await (s.SaveAsync(gavin));
			await (s.SaveAsync(emp));
			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IQuery namedQuery = s.GetNamedQuery("AllEmploymentAsMapped");
			IList list = await (namedQuery.ListAsync());
			Assert.AreEqual(1, list.Count);
			Employment emp2 = (Employment) list[0];
			Assert.AreEqual(emp2.EmploymentId, emp.EmploymentId);
			Assert.AreEqual(emp2.StartDate.Date, emp.StartDate.Date);
			Assert.AreEqual(emp2.EndDate, emp.EndDate);
			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IQuery sqlQuery = s.GetNamedQuery("EmploymentAndPerson");
			sqlQuery.SetResultTransformer(CriteriaSpecification.AliasToEntityMap);
			list = await (sqlQuery.ListAsync());
			Assert.AreEqual(1, list.Count);
			object res = list[0];
			AssertClassAssignability(res.GetType(), typeof(IDictionary));
			IDictionary m = (IDictionary) res;
			Assert.AreEqual(2, m.Count);
			await (t.CommitAsync());
			s.Close();

			if (TestDialect.SupportsDuplicatedColumnAliases)
			{
				s = OpenSession();
				t = s.BeginTransaction();
				sqlQuery = s.GetNamedQuery("organizationreturnproperty");
				sqlQuery.SetResultTransformer(CriteriaSpecification.AliasToEntityMap);
				list = await (sqlQuery.ListAsync());
				Assert.AreEqual(2, list.Count);
				m = (IDictionary) list[0];
				Assert.IsTrue(m.Contains("org"));
				AssertClassAssignability(m["org"].GetType(), typeof(Organization));
				Assert.IsTrue(m.Contains("emp"));
				AssertClassAssignability(m["emp"].GetType(), typeof(Employment));
				Assert.AreEqual(2, m.Count);
				await (t.CommitAsync());
				s.Close();
			}

			s = OpenSession();
			t = s.BeginTransaction();
			namedQuery = s.GetNamedQuery("EmploymentAndPerson");
			list = await (namedQuery.ListAsync());
			Assert.AreEqual(1, list.Count);
			object[] objs = (object[]) list[0];
			Assert.AreEqual(2, objs.Length);
			emp2 = (Employment) objs[0];
			gavin = (Person) objs[1];
			await (s.DeleteAsync(emp2));
			await (s.DeleteAsync(jboss));
			await (s.DeleteAsync(gavin));
			await (s.DeleteAsync(ifa));
			await (t.CommitAsync());
			s.Close();
		}

		[Test]
		public async Task AutoDetectAliasingAsync()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Organization ifa = new Organization("IFA");
			Organization jboss = new Organization("JBoss");
			Person gavin = new Person("Gavin");
			Employment emp = new Employment(gavin, jboss, "AU");
			await (s.SaveAsync(jboss));
			await (s.SaveAsync(ifa));
			await (s.SaveAsync(gavin));
			await (s.SaveAsync(emp));
			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IList list = await (s.CreateSQLQuery(EmploymentSQL)
			              .AddEntity(typeof(Employment).FullName)
			              .ListAsync());
			Assert.AreEqual(1, list.Count);

			Employment emp2 = (Employment) list[0];
			Assert.AreEqual(emp2.EmploymentId, emp.EmploymentId);
			Assert.AreEqual(emp2.StartDate.Date, emp.StartDate.Date);
			Assert.AreEqual(emp2.EndDate, emp.EndDate);

			s.Clear();

			list = await (s.CreateSQLQuery(EmploymentSQL)
			        .AddEntity(typeof(Employment).FullName)
			        .SetResultTransformer(CriteriaSpecification.AliasToEntityMap)
			        .ListAsync());
			Assert.AreEqual(1, list.Count);
			IDictionary m = (IDictionary) list[0];
			Assert.IsTrue(m.Contains("Employment"));
			Assert.AreEqual(1, m.Count);

			list = await (s.CreateSQLQuery(EmploymentSQL).ListAsync());
			Assert.AreEqual(1, list.Count);
			object[] o = (object[]) list[0];
			Assert.AreEqual(8, o.Length);

			list = await (s.CreateSQLQuery(EmploymentSQL).SetResultTransformer(CriteriaSpecification.AliasToEntityMap).ListAsync());
			Assert.AreEqual(1, list.Count);
			m = (IDictionary) list[0];
			Assert.IsTrue(m.Contains("EMPID") || m.Contains("empid"));
			Assert.IsTrue(m.Contains("AVALUE") || m.Contains("avalue"));
			Assert.IsTrue(m.Contains("ENDDATE") || m.Contains("enddate"));
			Assert.AreEqual(8, m.Count);

			// TODO H3: H3.2 can guess the return column type so they can use just addScalar("employerid"),
			// but NHibernate currently can't do it.
			list =
				await (s.CreateSQLQuery(EmploymentSQLMixedScalarEntity).AddScalar("employerid", NHibernateUtil.Int64).AddEntity(
					typeof(Employment)).ListAsync());
			Assert.AreEqual(1, list.Count);
			o = (object[]) list[0];
			Assert.AreEqual(2, o.Length);
			AssertClassAssignability(o[0].GetType(), typeof(long));
			AssertClassAssignability(o[1].GetType(), typeof(Employment));

			IQuery queryWithCollection = s.GetNamedQuery("organizationEmploymentsExplicitAliases");
			queryWithCollection.SetInt64("id", jboss.Id);
			list = await (queryWithCollection.ListAsync());
			Assert.AreEqual(list.Count, 1);

			s.Clear();

			list = await (s.CreateSQLQuery(OrganizationJoinEmploymentSQL)
			        .AddEntity("org", typeof(Organization))
			        .AddJoin("emp", "org.employments")
			        .ListAsync());
			Assert.AreEqual(2, list.Count);

			s.Clear();

			list = await (s.CreateSQLQuery(OrganizationFetchJoinEmploymentSQL)
			        .AddEntity("org", typeof(Organization))
			        .AddJoin("emp", "org.employments")
			        .ListAsync());
			Assert.AreEqual(2, list.Count);

			s.Clear();

			if (TestDialect.SupportsDuplicatedColumnAliases)
			{
				// TODO : why twice?
				await (s.GetNamedQuery("organizationreturnproperty").ListAsync());
				list = await (s.GetNamedQuery("organizationreturnproperty").ListAsync());
				Assert.AreEqual(2, list.Count);

				s.Clear();

				list = await (s.GetNamedQuery("organizationautodetect").ListAsync());
				Assert.AreEqual(2, list.Count);
			}

			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			await (s.DeleteAsync(emp2));

			await (s.DeleteAsync(jboss));
			await (s.DeleteAsync(gavin));
			await (s.DeleteAsync(ifa));
			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			Dimension dim = new Dimension(3, int.MaxValue);
			await (s.SaveAsync(dim));
			//		s.Flush();
			await (s.CreateSQLQuery("select d_len * d_width as surface, d_len * d_width * 10 as volume from Dimension").ListAsync());
			await (s.DeleteAsync(dim));
			await (t.CommitAsync());
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			SpaceShip enterprise = new SpaceShip();
			enterprise.Model = "USS";
			enterprise.Name = "Entreprise";
			enterprise.Speed = 50d;
			Dimension d = new Dimension(45, 10);
			enterprise.Dimensions = d;
			await (s.SaveAsync(enterprise));
			//		s.Flush();
			object[] result = (object[]) await (s.GetNamedQuery("spaceship").UniqueResultAsync());
			enterprise = (SpaceShip) result[0];
			Assert.IsTrue(50d == enterprise.Speed);
			Assert.IsTrue(450d == ExtractDoubleValue(result[1]));
			Assert.IsTrue(4500d == ExtractDoubleValue(result[2]));
			await (s.DeleteAsync(enterprise));
			await (t.CommitAsync());
			s.Close();
		}

		[Test]
		public async Task MixAndMatchEntityScalarAsync()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			Speech speech = new Speech();
			speech.Length = 23d;
			speech.Name = "Mine";
			await (s.SaveAsync(speech));
			await (s.FlushAsync());
			s.Clear();

			IList l = await (s.CreateSQLQuery("select name, id, flength, name as scalarName from Speech")
			           .SetResultSetMapping("speech")
			           .ListAsync());
			Assert.AreEqual(l.Count, 1);

			await (t.RollbackAsync());
			s.Close();
		}

		[Test]
		public async Task ParameterListAsync()
		{
			using (ISession s = OpenSession())
			{
				IList l = await (s.CreateSQLQuery("select id from Speech where id in (:idList)")
				           .AddScalar("id", NHibernateUtil.Int32)
				           .SetParameterList("idList", new int[] {0, 1, 2, 3}, NHibernateUtil.Int32)
				           .ListAsync());
			}
		}

		private double ExtractDoubleValue(object value)
		{
			if (value is double)
			{
				return (double) value;
			}
			else if (value is decimal)
			{
				return (double) (decimal) value;
			}
			else
			{
				return double.Parse(value.ToString());
			}
		}

		public static void AssertClassAssignability(System.Type source, System.Type target)
		{
			Assert.IsTrue(
				target.IsAssignableFrom(source),
				"Classes were not assignment-compatible : source<" +
				source.FullName +
				"> target<" +
				target.FullName + ">"
			);
		}

		class TestResultSetTransformer : IResultTransformer
		{
			public bool TransformTupleCalled { get; set; }
			public bool TransformListCalled { get; set; }

			public object TransformTuple(object[] tuple, string[] aliases)
			{
				this.TransformTupleCalled = true;
				return tuple;
			}

			public IList TransformList(IList collection)
			{
				this.TransformListCalled = true;
				return collection;
			}
		}

		[Test]
		public async Task CanSetResultTransformerOnFutureQueryAsync()
		{
			//NH-3222
			using (var s = this.OpenSession())
			using (s.BeginTransaction())
			{
				await (s.SaveAsync(new Person("Ricardo")));
				await (s.FlushAsync());

				var transformer = new TestResultSetTransformer();
				var l = s
					.CreateSQLQuery("select Name from Person")
					.SetResultTransformer(transformer)
					.Future<object[]>();

				Assert.AreEqual((await (l.GetEnumerableAsync())).Count(), 1);
				Assert.AreEqual("Ricardo", (await (l.GetEnumerableAsync())).ElementAt(0)[0]);
				Assert.IsTrue(transformer.TransformListCalled);
				Assert.IsTrue(transformer.TransformTupleCalled);
			}
		}

		[Test]
		public async Task CanSetResultTransformerOnFutureValueAsync()
		{
			//NH-3222
			using (var s = this.OpenSession())
			using (s.BeginTransaction())
			{
				await (s.SaveAsync(new Person("Ricardo")));
				await (s.FlushAsync());

				var transformer = new TestResultSetTransformer();
				var l = s
					.CreateSQLQuery("select Name from Person")
					.SetResultTransformer(transformer)
					.FutureValue<object[]>();

				var v = await (l.GetValueAsync());

				Assert.AreEqual("Ricardo", v[0]);
				Assert.IsTrue(transformer.TransformListCalled);
				Assert.IsTrue(transformer.TransformTupleCalled);
			}
		}

		[Test]
		public async Task CanExecuteFutureListAsync()
		{
			//NH-3222
			using (var s = this.OpenSession())
			using (s.BeginTransaction())
			{
				await (s.SaveAsync(new Person("Ricardo")));
				await (s.FlushAsync());

				var l = s
					.CreateSQLQuery("select Name from Person")
					.Future<string>();

				Assert.AreEqual((await (l.GetEnumerableAsync())).Count(), 1);
				Assert.AreEqual("Ricardo", (await (l.GetEnumerableAsync())).ElementAt(0));
			}
		}

		[Test]
		public async Task CanExecuteFutureValueAsync()
		{
			//NH-3222
			using (var s = this.OpenSession())
			using (s.BeginTransaction())
			{
				await (s.SaveAsync(new Person("Ricardo")));
				await (s.FlushAsync());

				var l = s
					.CreateSQLQuery("select Name from Person")
					.FutureValue<string>();

				var v = await (l.GetValueAsync());

				Assert.AreEqual("Ricardo", v);
			}
		}

		[Test]
		public async Task HandlesManualSynchronizationAsync()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				s.SessionFactory.Statistics.IsStatisticsEnabled = true;
				s.SessionFactory.Statistics.Clear();

				// create an Organization...
				Organization jboss = new Organization("JBoss");
				await (s.PersistAsync(jboss));

				// now query on Employment, this should not cause an auto-flush
				await (s.CreateSQLQuery(EmploymentSQL).AddSynchronizedQuerySpace("ABC").ListAsync());
				Assert.AreEqual(0, s.SessionFactory.Statistics.EntityInsertCount);

				// now try to query on Employment but this time add Organization as a synchronized query space...
				await (s.CreateSQLQuery(EmploymentSQL).AddSynchronizedEntityClass(typeof(Organization)).ListAsync());
				Assert.AreEqual(1, s.SessionFactory.Statistics.EntityInsertCount);

				// clean up
				await (s.DeleteAsync(jboss));
				await (s.Transaction.CommitAsync());
				s.Close();
			}
		}
	}
}
