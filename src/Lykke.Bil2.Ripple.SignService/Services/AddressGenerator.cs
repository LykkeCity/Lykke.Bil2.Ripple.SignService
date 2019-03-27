using System.Security.Cryptography;
using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.SignService.Requests;
using Lykke.Bil2.Contract.SignService.Responses;
using Lykke.Bil2.Sdk.SignService.Models;
using Lykke.Bil2.Sdk.SignService.Services;
using Lykke.Bil2.Sdk.Exceptions;
using Ripple.Address;

namespace Lykke.Bil2.Ripple.SignService.Services
{
    public class AddressGenerator : IAddressGenerator
    {
        public Task<AddressCreationResult> CreateAddressAsync()
        {
            throw new OperationNotSupportedException();
        }

        public Task<CreateAddressTagResponse> CreateAddressTagAsync(string address, CreateAddressTagRequest request)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new RequestValidationException("Invalid address, must be non empty",
                    nameof(address));
            }

            if (!AddressCodec.IsValidAddress(address))
            {
                throw new RequestValidationException("Invalid address, must be valid Ripple address",
                    nameof(address));
            }

            if (request.Type != AddressTagType.Number)
            {
                throw new RequestValidationException($"Invalid tag type, must be {AddressTagType.Number}",
                    nameof(CreateAddressTagRequest.Type));
            }

            var buffer = new byte[4];

            RandomNumberGenerator.Create()
                .GetBytes(buffer);

            var tag = BitConverter.ToUInt32(buffer, 0)
                .ToString();

            return Task.FromResult(new CreateAddressTagResponse(tag));
        }
    }
}
