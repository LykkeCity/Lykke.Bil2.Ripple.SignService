using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.SignService.Requests;
using Lykke.Bil2.Ripple.SignService.Services;
using NUnit.Framework;

namespace Tests
{
    public class AddressGeneratorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ShouldGenerateRandomNumericTags()
        {
            // Arrange

            var addressGenerator = new AddressGenerator();
            var tags = new List<string>();

            // Act

            for (var i = 0; i < 10; i++)
            {
                var res = await addressGenerator.CreateAddressTagAsync("rK3AmAaXGZsnJTuQ7hdBkFb8S6R3v7MAaG", new CreateAddressTagRequest(type: AddressTagType.Number));
                tags.Add(res.Tag);
            }

            // Assert

            Assert.That(tags.Distinct().Count() == tags.Count);
            Assert.DoesNotThrow(() => tags.Select(t => uint.Parse(t)).ToList());
        }

        [Test]
        public void ShouldValidateTagType()
        {
            // Arrange

            var addressGenerator = new AddressGenerator();

            // Act & Assert

            Assert.ThrowsAsync<RequestValidationException>(async () =>
                await addressGenerator.CreateAddressTagAsync("rK3AmAaXGZsnJTuQ7hdBkFb8S6R3v7MAaG", new CreateAddressTagRequest(type: AddressTagType.Text)));

            Assert.DoesNotThrowAsync(async () =>
                await addressGenerator.CreateAddressTagAsync("rK3AmAaXGZsnJTuQ7hdBkFb8S6R3v7MAaG", new CreateAddressTagRequest(type: AddressTagType.Number)));
        }

        [Test]
        public void ShouldValidateAddress_Invalid([Values("qwe", "", null)] string address)
        {
            // Arrange

            var addressGenerator = new AddressGenerator();

            // Act & Assert

            Assert.ThrowsAsync<RequestValidationException>(async () =>
                await addressGenerator.CreateAddressTagAsync(address, new CreateAddressTagRequest(type: AddressTagType.Number)));
        }

        [Test]
        public void ShouldValidateAddress_DifferentNetworks([Values("rK3AmAaXGZsnJTuQ7hdBkFb8S6R3v7MAaG", "rHb9CJAWyB4rj91VRWn96DkukG4bwdtyTh")] string address)
        {
            // Arrange

            var addressGenerator = new AddressGenerator();

            // Act & Assert

            Assert.DoesNotThrowAsync(async () =>
                await addressGenerator.CreateAddressTagAsync(address, new CreateAddressTagRequest(type: AddressTagType.Number)));
        }
    }
}