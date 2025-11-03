using CaffePOS.Model;
using CaffePOS.Model.DTOs.Requests;
using CaffePOS.Model.DTOs.Response;
using CaffePOS.Services;
using Microsoft.AspNetCore.Mvc;

namespace CaffePOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(PaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<PaymentResponseDto>>> GetAllPayment()
        {
            try
            {
                var payments = await _paymentService.GetAllPayment();
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi lay toan bo cac cach thanh toan.");
                return StatusCode(500, "Da co loi xay tr khi lay toan bo phuong thuc thanh toan");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] Model.DTOs.Response.PaymentPostDto dto)
        {
            try
            {
                if(dto == null)
                {
                    return BadRequest("Du lieu khong hop le.");
                }
                var payment = new Payments
                {
                    OrderId = dto.order_id,
                    PaymentDate = DateTime.Now,
                    Amount = dto.amount,
                    Method = dto.method,
                    TransactionId = dto.transaction_id,
                    Notes = dto.notes
                };
                await _paymentService.CreatePayment(dto);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi tao phuong thuc thanh toan.");
                return StatusCode(500, "Da co loi xay ra khi tao phuong thuc thanh toan");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentResponseDto>> Detail(int id)
        {
            try
            {
                var order = await _paymentService.Detail(id);
                if(order == null)
                {
                    return NotFound($"Khong tim thay phuong thuc tanh toan");
                }
                return Ok(order);
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Khong tim thay chi tiet thanh toan");
                return StatusCode(500, "Da co loi khi lay chi tiet thanh toan");
            }
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> EditPayment(int id, [FromBody] PaymentPostDto dto)
        {
            try
            {
                if(dto == null)
                {
                    return BadRequest("Du lieu khong hop le");
                }
                var updatePayment = await _paymentService.EditPayment(id, dto);
                if(updatePayment == null)
                {
                    return NotFound($"Khong tim thay du lieu san pham");
                }
                return Ok(updatePayment);
            } catch (Exception ex)
            {
                _logger.LogError(ex, $"Loi khi cap nhat phuong thuc thanh toan");
                return StatusCode(500, "Da co loi xay ra khi cap nhat phuong thuc thaanh toan");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            try
            {
                var result = await _paymentService.DeletePayment(id);
                if(!result)
                {
                    return NotFound("Khong tim thay phuong thuc thanh toan");
                }
                return Ok("Xoa phuong thuc thanh toan thanh cong");
            } catch(Exception ex)
            {
                _logger.LogError(ex, $"Co loi khi xoa phuong thuc thanh toan");
                return StatusCode(500, "Da co loi khi xoa phuong thuc thanh toan");
            }
        }
        [HttpPost("process-payment")]
        public async Task<ActionResult<ProcessPaymentResponseDto>> ProcessPayment([FromBody] ProcessPaymentRequestDto request)
        {
            try
            {
                var response = await _paymentService.ProcessPayment(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi xu ly thanh toan.");
                return StatusCode(500, "Da co loi xay ra khi xu ly thanh toan.");
            }
        }
    }
}
