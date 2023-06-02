using Dapper;
using WebApi.Context;
using WebApi.Dtos;

namespace WebApi.Services;

public class QuoteService
{
    private readonly DapperContext _context;
    private readonly IFileService _fileService;

    public QuoteService(DapperContext context,IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<List<GetQuoteDto>> GetQuotes()
    {
        using (var conn = _context.CreateConnection())
        {
            var sql = "select id as Id  , author as Author,quotetext as quoteText,categoryid as CategoryId,file_name as filename from quotes;";
            var result = await conn.QueryAsync<GetQuoteDto>(sql);
            return result.ToList();
        }
    }

    public async Task<GetQuoteDto> AddQuote(AddQuoteDto quote)
    {
        using (var conn = _context.CreateConnection())
        {
            //upload file
            var filename = _fileService.CreateFile("images", quote.File);
            var sql = "insert into quotes (author, quotetext, categoryid, file_name) VALUES (@author, @QuoteText, @CategoryId, @filename) returning id;";
            var result =  conn.ExecuteScalar<int>(sql,new
            {
                quote.Author,
                quote.QuoteText,
                quote.CategoryId,
                filename
            });
            return new GetQuoteDto()
            {
                Author = quote.Author,
                QuoteText = quote.QuoteText,
                CategoryId = quote.CategoryId,
                FileName = filename,
                Id = result
            };
        }
    }

    public async Task<GetQuoteDto> UpdateQuote(AddQuoteDto quote)
    {
        using (var conn = _context.CreateConnection())
        {
            var existing =
                conn.QuerySingleOrDefault<GetQuoteDto>(
                    "select id as Id, author as Author,quotetext as QuoteText,categoryid as CategoryId,file_name as filename from quotes where id=@id;",
                    new {quote.Id});
            if (existing == null)
            {
                return new GetQuoteDto();
            }

            string filename = null;
            // if file not found on database ->just add
            //if file found in database and file is found in quote -> delete file in database and add new file
            //if file found in database and not found in quote -> just do nothing
            if (quote.File != null && existing.FileName != null)
            {
                _fileService.DeleteFile("images", existing.FileName);
                filename = _fileService.CreateFile("images", quote.File);
            }
            else if (quote.File != null && existing.FileName == null)
            {
                filename = _fileService.CreateFile("images", quote.File);
            }
            var sql = "update quotes set author=@Author, quotetext=@QuoteText,categoryid=@CategoryId  where Id=@Id";
            if (quote.File != null)
            {
                sql =
                    "update quotes set author=@Author, quotetext=@QuoteText,categoryid=@CategoryId,file_name=@FileName where id=@Id";
            }
            var result =  conn.Execute(sql,new
            {
                quote.Author,
                quote.QuoteText,
                quote.CategoryId,
                filename,
                quote.Id
            });
            return new GetQuoteDto()
            {
                Author = quote.Author,
                QuoteText = quote.QuoteText,
                CategoryId = quote.CategoryId,
                FileName = filename,
                Id = result
            };
        }
    }
}