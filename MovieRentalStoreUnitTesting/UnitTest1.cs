using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieRentalStore;

namespace MovieRentalStoreUnitTesting
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestConnection()
        {
            // Arrange
            var conn = new Connection();

            // Act
            var actual = conn.GetConn();

            // Assert
            Assert.AreEqual(@"Data Source=DESKTOP-H9RU0PL; Initial Catalog=RentingStore; Integrated Security=True", actual);
        }

        [TestMethod]
        public void Check_Rented_Movies()
        {
            // Arrange
            var data = new Database();

            // Act
            var actual = data.HasCopiesRented(0);

            // Assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void Check_User_Has_Movies_Rented()
        {
            // Arrange
            var data = new Database();

            // Act
            var actual = data.UserHasRentedMovies(0);

            // Assert
            Assert.AreEqual(false, actual);
        }
    }
}
