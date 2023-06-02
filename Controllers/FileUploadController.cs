
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Services;

[ApiController]
[Route("[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly QuoteService _quoteService;

    public FileUploadController(IWebHostEnvironment webHostEnvironment, QuoteService quoteService)
    {
        _webHostEnvironment = webHostEnvironment;
        _quoteService = quoteService;
    }

    [HttpGet("GetList")]
    public async Task<List<GetQuoteDto>> GetListOfFiles()
    {
        return await _quoteService.GetQuotes();
    }

    [HttpPost("Add")]
    public async Task<GetQuoteDto> UploadFile([FromForm] AddQuoteDto quote)
    {
        return await _quoteService.AddQuote(quote);
    }

    
    [HttpPut("Update")]
    public async Task<GetQuoteDto> Update([FromForm] AddQuoteDto quote)
    {
        return await _quoteService.UpdateQuote(quote);
    }


}   