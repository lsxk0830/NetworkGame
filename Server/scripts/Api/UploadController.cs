using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly string _uploadPath;
    private readonly AppDbContext _dbContext;

    public UploadController(AppDbContext dbContext, IHostEnvironment env)
    {
        _dbContext = dbContext;
        _uploadPath = Path.Combine(env.ContentRootPath, "Uploads");
        Directory.CreateDirectory(_uploadPath);
    }

    [HttpPost]
    [RequestSizeLimit(10_000_000)] // 限制10MB文件大小
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        // 验证文件类型
        var allowedTypes = new Dictionary<string, string>
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png"
        };
        if (!allowedTypes.TryGetValue(file.ContentType, out var ext))
            return BadRequest("仅支持JPEG/PNG格式");

        // 生成安全文件名
        var fileName = $"{Guid.NewGuid()}{ext}";
        var savePath = Path.Combine(_uploadPath, fileName);

        // 保存文件
        using (var stream = new FileStream(savePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { fileName });
    }

    [HttpGet("images/{id}")]
    public IActionResult GetImage(string id)
    {
        var file = Directory.GetFiles(_uploadPath, $"{id}.*").FirstOrDefault();
        if (file == null) return NotFound();

        return PhysicalFile(file, $"image/{Path.GetExtension(file)[1..]}");
    }

    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.AvatarPath)
            .FirstOrDefaultAsync(u => u.ID == id);

        return user != null
            ? Ok(new { user.ID, user.Name, AvatarUrl = $"/api/images/{user.AvatarPath}" })
            : NotFound();
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1)
    {
        const int pageSize = 10;
        if (page < 1) return BadRequest("页码必须大于0");

        var users = await _dbContext.Users
            .OrderBy(u => u.ID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new
            {
                u.ID,
                u.Name,
                AvatarUrl = $"/api/images/{u.AvatarPath}"
            }).ToListAsync();

        return Ok(new { Total = _dbContext.Users.Count(), Data = users });
    }
}

// AppDbContext.cs
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}