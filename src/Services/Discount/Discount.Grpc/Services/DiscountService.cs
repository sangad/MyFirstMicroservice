using AutoMapper;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;
        public DiscountService(IDiscountRepository discountRepository, IMapper mapper, ILogger<DiscountService> logger)
        {
            _discountRepository = discountRepository;
            _mapper = mapper;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _discountRepository.GetDiscount(request.ProductName);

            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Disocunt with ProductName={request.ProductName} is not found"));
            }
            _logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount : {amount}", coupon.ProductName, coupon.Amount);
            var couponModel=_mapper.Map<CouponModel>(coupon);

            return couponModel;
        }
    }
}
