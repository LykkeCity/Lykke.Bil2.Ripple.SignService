using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.Ripple.SignService.Services;
using NUnit.Framework;

namespace Lykke.Bil2.Ripple.SignService.Tests
{
    public class TransactionSignerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task ShouldSignTransaction()
        {
            // Arrange

            var transactionSigner = new TransactionSigner();
            var privateKey = "shv6CbpPSwBsrvQcXjtJGBDRFWC14";
            var tx = @"{
                'Account': 'rE6jo1LZNZeD3iexQ6DnfCREEWZ9aUweVy',
                'Amount': '1000000',
                'Destination': 'rHjZWtWZd6MZrF8zJig2JAz9vmwAHpiSrU',
                'DestinationTag': 0,
                'Fee': '12',
                'Flags': 2147483648,
                'Sequence': 177,
                'LastLedgerSequence': 17162100,
                'TransactionType' : 'Payment'
            }";

            // Act

            var signed = await transactionSigner.SignAsync(new [] { privateKey }, tx.ToBase58());

            // Assert

            Assert.AreEqual(
                signed.TransactionId,
                "D1DEE34D71B2E063BDB24FF35B3E5F06E8F38DA2987AEE734F6EFF8339AE3BB6");
        }

        [Test]
        public void ShouldValidateTransaction_InvalidJson()
        {
            // Arrange

            var transactionSigner = new TransactionSigner();
            var privateKey = "shv6CbpPSwBsrvQcXjtJGBDRFWC14";
            var txMissedComma = @"{
                'Account': 'rE6jo1LZNZeD3iexQ6DnfCREEWZ9aUweVy',
                'Amount': '1000000',
                'Destination': 'rHjZWtWZd6MZrF8zJig2JAz9vmwAHpiSrU',
                'DestinationTag': 0,
                'Fee': '12',
                'Flags': 2147483648,
                'Sequence': 177,
                'LastLedgerSequence': 17162100
                'TransactionType' : 'Payment'
            }";

            // Act & Assert

            Assert.ThrowsAsync<RequestValidationException>(async () =>
                await transactionSigner.SignAsync(new [] { privateKey }, txMissedComma.ToBase58()));
        }

        [Test]
        public void ShouldValidateTransaction_InvalidTxFormat()
        {
            // Arrange

            var transactionSigner = new TransactionSigner();
            var privateKey = "shv6CbpPSwBsrvQcXjtJGBDRFWC14";
            var txInvalidAddress = @"{
                'Account': 'qwe',
                'Amount': '1000000',
                'Destination': 'rHjZWtWZd6MZrF8zJig2JAz9vmwAHpiSrU',
                'DestinationTag': 0,
                'Fee': '12',
                'Flags': 2147483648,
                'Sequence': 177,
                'LastLedgerSequence': 17162100,
                'TransactionType' : 'Payment'
            }";

            // Act & Assert

            Assert.ThrowsAsync<RequestValidationException>(async () =>
                await transactionSigner.SignAsync(new [] { privateKey }, txInvalidAddress.ToBase58()));
        }
    }
}