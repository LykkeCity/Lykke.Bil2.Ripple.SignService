using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.Contract.SignService.Responses;
using Lykke.Bil2.Sdk.SignService.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ripple.Signing;
using Ripple.TxSigning;

namespace Lykke.Bil2.Ripple.SignService.Services
{
    public class TransactionSigner : ITransactionSigner
    {
        public Task<SignTransactionResponse> SignAsync(IReadOnlyCollection<string> privateKeys, Base58String requestTransactionContext)
        {
            if (!privateKeys.Any() ||
                !privateKeys.All(k => ValidateKeyFormat(k)))
            {
                throw new RequestValidationException("Invalid private key(s)",
                    nameof(privateKeys));
            }

            // TODO: switch to HEX instead of JSON if possible on TransactionExecutor side

            JObject txJson;

            try
            {
                txJson = JObject.Parse(requestTransactionContext.DecodeToString());
            }
            catch (JsonReaderException ex)
            {
                throw new RequestValidationException("Invalid transaction context, must be valid JSON",
                    ex, nameof(requestTransactionContext));
            }

            SignedTx signedTx;

            try
            {
                signedTx = TxSigner.SignJson(txJson, privateKeys.First());
            }
            catch (InvalidTxException ex)
            {
                throw new RequestValidationException("Invalid transaction context, must be valid Ripple transaction",
                    ex, nameof(requestTransactionContext));
            }

            return Task.FromResult(new SignTransactionResponse
            (
                signedTx.TxBlob.ToBase58(),
                signedTx.Hash
            ));
        }

        public bool ValidateKeyFormat(string privateKey)
        {
            try
            {
                var keyPair = Seed.FromBase58(privateKey).KeyPair();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
