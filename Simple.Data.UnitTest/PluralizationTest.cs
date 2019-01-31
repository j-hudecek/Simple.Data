#if(!MONO)
namespace Simple.Data.UnitTest
{
    using System.Globalization;
    using Extensions;
    using NUnit.Framework;

    [TestFixture]
    public class PluralizationTest
    {
        [OneTimeSetUp]
        public void FixtureSetup()
        {
            Database.SetPluralizer(new EntityPluralizer());
        }

        /// <summary>
        ///A test for IsPlural
        ///</summary>
        [Test]
        public void IsPluralLowercaseUsersShouldReturnTrue()
        {
            Assert.IsTrue("Users".IsPlural());
        }

        /// <summary>
        ///A test for IsPlural
        ///</summary>
        [Test]
        public void IsPluralUppercaseUsersShouldReturnTrue()
        {
            Assert.IsTrue("USERS".IsPlural());
        }

        /// <summary>
        ///A test for IsPlural
        ///</summary>
        [Test]
        public void IsPluralLowercaseUserShouldReturnFalse()
        {
            Assert.IsFalse("User".IsPlural());
        }

        /// <summary>
        ///A test for IsPlural
        ///</summary>
        [Test]
        public void IsPluralUppercaseUserShouldReturnFalse()
        {
            Assert.IsFalse("USER".IsPlural());
        }

        /// <summary>
        ///A test for Pluralize
        ///</summary>
        [Test()]
        public void PluralizeUserShouldReturnUsers()
        {
            Assert.AreEqual("Users", "User".Pluralize());
        }

        /// <summary>
        ///A test for Pluralize
        ///</summary>
        [Test()]
        // ReSharper disable InconsistentNaming
        public void PluralizeUSERShouldReturnUSERS()
        // ReSharper restore InconsistentNaming
        {
            Assert.AreEqual("USERS", "USER".Pluralize());
        }

        /// <summary>
        ///A test for Singularize
        ///</summary>
        [Test()]
        public void SingularizeUsersShouldReturnUser()
        {
            Assert.AreEqual("User", "Users".Singularize());
        }

        /// <summary>
        ///A test for Singularize
        ///</summary>
        [Test()]
        public void SingularizeUserShouldReturnUser()
        {
            Assert.AreEqual("User", "User".Singularize());
        }

        /// <summary>
        ///A test for Singularize
        ///</summary>
        [Test()]
        // ReSharper disable InconsistentNaming
        public void SingularizeUSERSShouldReturnUSER()
        // ReSharper restore InconsistentNaming
        {
            Assert.AreEqual("USER", "USERS".Singularize());
        }

        [Test]
        public void PluralizeCompanyShouldReturnCompanies()
        {
            Assert.AreEqual("Companies", "Company".Pluralize());
        }

        [Test]
        public void SingularizeCompaniesShouldReturnCompany()
        {
            Assert.AreEqual("Company", "Companies".Singularize());
        }
    }

    class EntityPluralizer : IPluralizer
    {
        private readonly Pluralize.NET.Core.Pluralizer _pluralizationService = new Pluralize.NET.Core.Pluralizer();

        public bool IsPlural(string word)
        {
            return (_pluralizationService.Pluralize(word) == word);
        }

        public bool IsSingular(string word)
        {
            return (_pluralizationService.Singularize(word) == word); ;
        }

        public string Pluralize(string word)
        {
            bool upper = (word.IsAllUpperCase());
            word = _pluralizationService.Pluralize(word);
            return upper ? word.ToUpper() : word;
        }

        public string Singularize(string word)
        {
            return _pluralizationService.Singularize(word);
        }
    }
}
#endif