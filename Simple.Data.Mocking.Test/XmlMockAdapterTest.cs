﻿// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Simple.Data.Mocking.Test
{
    /// <summary>
    ///This is a test class for XmlStubAdapterTest and is intended
    ///to contain all XmlStubAdapterTest Unit Tests
    ///</summary>
    [TestFixture]
    public class XmlMockAdapterTest
    {
        private XmlMockAdapter _mockAdapter;

        [OneTimeSetUp]
        public void MyTestInitialize()
        {
            _mockAdapter =
                new XmlMockAdapter(
                    @"<Root><Users _keys=""Id"" Id=""System.Int32"" Key=""System.Guid"">
<User Id=""1"" Email=""foo"" Password=""bar"" Key=""4A1c8a8a-238d-443e-8ab2-bdf046a91fd7"">
  <Pets><Pet Name=""Fido""/></Pets>
</User>
<User Id=""2"" Email=""bar"" Password=""quux""/>
<User Id=""3"" Email=""baz"" Password=""quux""/>
<User Id=""4"" Email=""baz"" Password=""quux""/>
</Users></Root>");
            MockHelper.UseMockAdapter(_mockAdapter);
        }

        /// <summary>
        ///A test for Find
        ///</summary>
        [Test]
        public void FindByEmail_ShouldFindRecord()
        {
            dynamic user = Database.Default.Users.FindByEmail("foo");
            Assert.AreEqual(1, user.Id);
            Assert.AreEqual("foo", user.Email);
            Assert.AreEqual("bar", user.Password);
        }

        /// <summary>
        ///A test for Find
        ///</summary>
        [Test]
        public void SeparateThreads_Should_SeeDifferentMocks()
        {
            int r1 = 0;
            int r2 = 0;

            var t1 = new Thread(() => r1 = ThreadTestHelper(1));
            var t2 = new Thread(() => r2 = ThreadTestHelper(2));
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();

            Assert.AreEqual(1, r1);
            Assert.AreEqual(2, r2);
        }

        private static int ThreadTestHelper(int userId)
        {
            var mockAdapter =
                new XmlMockAdapter(
                    @"<Root><Users _keys=""Id"" Id=""System.Int32"" Key=""System.Guid"">
<User Id=""" + userId + @""" Email=""foo"" Password=""bar""/>
</Users></Root>");
            MockHelper.UseMockAdapter(mockAdapter);
            return Database.Default.Users.FindByEmail("foo").Id;
        }


        /// <summary>
        ///A test for Find
        ///</summary>
        [Test]
        public void FindByKey_ShouldFindRecord()
        {
            var key = Guid.Parse("4a1c8a8a-238d-443e-8ab2-bdf046a91fd7");
            dynamic user = Database.Default.Users.FindByKey(key);
            Assert.IsNotNull(user);
            Assert.AreEqual(1, user.Id);
            Assert.AreEqual("foo", user.Email);
            Assert.AreEqual("bar", user.Password);
        }

        /// <summary>
        ///A test for Find
        ///</summary>
        [Test]
        public void FindById_ShouldFindRecord()
        {
            dynamic user = Database.Default.Users.FindById(1);
            Assert.AreEqual(1, user.Id);
        }

        [Test]
        public void UserShouldHavePet()
        {
            dynamic user = Database.Default.Users.FindById(1);
            Assert.IsNotNull(user.Pets);
        }

        [Test]
        public void All_ShouldReturnTwoUsers()
        {
            IEnumerable<object> users = Database.Default.Users.All().Cast<object>();
            Assert.AreEqual(_mockAdapter.Data.Element("Users").Elements().Count(), users.Count());
        }

        [Test]
        public void TestUpdateBy()
        {
            int updated = Database.Default.Users.UpdateById(Id: 1, Email: "quux");
            Assert.AreEqual(1, updated);
            var element = _mockAdapter.Data.Element("Users").Elements().Where(e => e.Attribute("Id") != null && e.Attribute("Id").Value == "1").Single();
            Assert.AreEqual("quux", element.Attribute("Email").Value);
        }

        [Test]
        public void TestUpdate()
        {
            dynamic record = new SimpleRecord();
            record.Id = 4;
            record.Email = "quux";
            Database.Default.Users.Update(record);
            var element = _mockAdapter.Data.Element("Users").Elements().Where(e => e.Attribute("Id") != null && e.Attribute("Id").Value == "4").Single();
            Assert.AreEqual("quux", element.Attribute("Email").Value);
        }

        [Test]
        public void TestDelete()
        {
            int deleted = Database.Default.Users.Delete(Id: 2);
            Assert.AreEqual(1, deleted);
            var element = _mockAdapter.Data.Element("Users").Elements().Where(e => e.Attribute("Id") != null && e.Attribute("Id").Value == "2").SingleOrDefault();
            Assert.IsNull(element);
        }

        [Test]
        public void TestDeleteBy()
        {
            int deleted = Database.Default.Users.DeleteById(3);
            Assert.AreEqual(1, deleted);
            var element = _mockAdapter.Data.Element("Users").Elements().Where(e => e.Attribute("Id") != null && e.Attribute("Id").Value == "3").SingleOrDefault();
            Assert.IsNull(element);
        }

        [Test]
        public void TestInsert()
        {
            var row = Database.Default.Users.Insert(Id: 5, Email: "bob", Password: "secret");
            Assert.AreEqual(5, row.Id);
            Assert.AreEqual("bob", row.Email);
            Assert.AreEqual("secret", row.Password);
            var element = _mockAdapter.Data.Element("Users").Elements().Where(e => e.Attribute("Id") != null && e.Attribute("Id").Value == "5").SingleOrDefault();
            Assert.IsNotNull(element);
            Assert.AreEqual("5", element.Attribute("Id").Value);
            Assert.AreEqual("bob", element.Attribute("Email").Value);
            Assert.AreEqual("secret", element.Attribute("Password").Value);
        }

        [Test]
        public void IsValidRelation_Users_Pets_ShouldReturnTrue()
        {
            Assert.IsTrue(_mockAdapter.IsValidRelation("Users", "Pets"));
        }

        [Test]
        public void Users_Pets_ShouldReturn_OneRow_WithName_Fido()
        {
            IEnumerable<dynamic> pets = Database.Default.Users.FindById(1).Pets;
            Assert.AreEqual(1, pets.Count());
            Assert.AreEqual("Fido", pets.Single().Name);
        }

        [Test]
        public void FindBy_WithNonMatchingSecondValue_ShouldNotFind()
        {
            var adapter = new XmlMockAdapter(
            @"<root>
                <Users>
                    <User Email=""someUser"" Password=""PASSWORD"" UserType=""AccountExecutive""/>
                </Users>          
                <CustomerData SiteNo=""System.Int32"" CustomerNo=""System.Int32"">
                    <Customer SiteNo=""500"" CustomerNo=""1"" SiteName=""Trump Towers"" EmailCust=""customerEmail"" AccountManagerEmail=""someOtherAccountExecutive""/>
                </CustomerData>      
            </root>");
            dynamic db = new Database(adapter);
            var customer = db.CustomerData.FindByCustomerNoAndAccountManagerEmail(1, "someEmail");
            Assert.IsNull(customer);
        }

        [Test]
        public void FindWithJoinInCriteriaSHouldWork()
        {
            var adapter = new XmlMockAdapter(@"<root><Users UserKey=""System.Guid"" UserCustomers=""UserCustomers"">
                        <User Email=""somesiteUser"" Password=""PASSWORD"" UserType=""Customer"" UserKey=""FF47BE0F-A6AE-4B52-B7CC-B2F3CA413838"">
                            <UserCustomers UserKey=""System.Guid"" CustomerNo=""System.Int32"" SiteNo=""System.Int32"">
                                <UserCustomer UserKey=""FF47BE0F-A6AE-4B52-B7CC-B2F3CA413838"" CustomerNo=""1000"" SiteNo=""501""/>
                            </UserCustomers>
                        </User>
                    </Users></root>");
            dynamic db = new Database(adapter);

            var associatedUser =
                db.Users.Find(db.Users.UserCustomers.CustomerNo == 1000 && db.Users.UserType == "Customer");

            Assert.IsNotNull(associatedUser);
            Assert.AreEqual(new Guid("FF47BE0F-A6AE-4B52-B7CC-B2F3CA413838"), associatedUser.UserKey);
        }

        [Test]
        public void FindByUrlId_ShouldFindRecord()
        {
            var adapter = new XmlMockAdapter(@"<Root>
                   <Urls _keys=""UrlId"" UrlId=""System.Int32"" >
                       <Url UrlId=""1"" LongUrl=""http://www.somesite.tld/"" Hash=""Hm5zT89z""/>
                   </Urls>
                 </Root>");

            dynamic db = new Database(adapter);

            var url = db.Urls.FindByUrlId(1);
            Assert.IsNotNull(url);
            Assert.AreEqual("http://www.somesite.tld/", url.LongUrl);
            Assert.AreEqual("Hm5zT89z", url.Hash);
        }
    }
}
// ReSharper restore InconsistentNaming