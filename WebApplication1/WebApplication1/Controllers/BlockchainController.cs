using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ShipmentApp.Services;
using Newtonsoft.Json;

namespace WebApplication1.Controllers
{
    [RoutePrefix("api/blockchain")]
    public class BlockchainController : ApiController
    {
        private readonly BlockchainService _blockchainService;

        public BlockchainController()
        {
            _blockchainService = new BlockchainService();
        }

        public class ShipmentUpdateRequest
        {
            public string TrackingId { get; set; }
            public string DataHash { get; set; }
        }

        public class ShipmentUpdateResponse
        {
            public string TransactionHash { get; set; }
            public long? BlockNumber { get; set; }
        }

        [HttpPost]
        [Route("addShipmentUpdate")]
        public async Task<IHttpActionResult> AddShipmentUpdate(ShipmentUpdateRequest request)
        {
            if (string.IsNullOrEmpty(request.TrackingId) || string.IsNullOrEmpty(request.DataHash))
            {
                return BadRequest("TrackingId and DataHash are required");
            }

            try
            {
                var transactionHash = await _blockchainService.AddShipmentUpdateAsync(request.TrackingId, request.DataHash);
                
                return Ok(new ShipmentUpdateResponse
                {
                    TransactionHash = transactionHash,
                    // BlockNumber is not immediately available in Ethereum transactions
                    // It would require a separate call to get the receipt after the transaction is mined
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("getShipmentHistory")]
        public async Task<IHttpActionResult> GetShipmentHistory(string trackingId)
        {
            if (string.IsNullOrEmpty(trackingId))
            {
                return BadRequest("TrackingId is required");
            }

            try
            {
                var history = await _blockchainService.GetShipmentHistoryAsync(trackingId);
                return Ok(new { history });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}