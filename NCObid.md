can you help me with full code of ProductRepository , ProductService (and maybe extra AuctionService if needed) , DTOs, VM, Controller (Product or maybe one more for Auction)
RazorView for ImportExcel and ViewProduct for update, in process of upload excel for product list/template and publish / view to buyer for submit a price over existing min provided?
optimaze downbelow methods and update to complete proccess, I will update to project and check function
STEP 1 -> Excel (fixed template)
Solution
│
├── Domain
│   └── Entities (Product, Bid, ...)
│
├── Application
│   ├── DTOs
        └── ProductExcelImportDto
        └── ImportResultDto
│   ├── Interfaces
│   └── Services
        └── ProductImportService
        └── ProductService
        └── AuctionService
│
├── Infrastructure
│   ├── Data
│   │   ├── AppDbContext.cs
│   │   └── Repositories
│   │       └── ProductRepository.cs
│   │
│   ├── Excel
│   │   └── ExcelParser.cs   ✅ HERE
│   │
│   └── DependencyInjection.cs
│
└── Web
    ├── Controllers
        └── ProductController (Import + Admin view)
        └── AuctionController (Buyer bidding)
    ├── ViewModels
        └── UploadExcelViewModel
        └── ProductListVM (Buyer view)
        └── PlaceBidVM
    └── Views
        
// Excel parsing helper (using EPPlus or ClosedXML)
public class ExcelParser
{
    public List<ProductImportViewModel> Parse(Stream excelStream)
    {
        using var package = new ExcelPackage(excelStream);
        var worksheet = package.Workbook.Worksheets[0];
        var products = new List<ProductImportViewModel>();
        for (int row = 2; row <= worksheet.Dimension.Rows; row++) // Skip header
        {
            var importModel = new ProductImportViewModel
            {
                ProductCode = worksheet.Cells[row, 1].Text,
                ProductCategory = worksheet.Cells[row, 2].Text,
                // Map other columns...
            };
            products.Add(importModel);
        }
        return products;
    }
}
    ↓ (binds to)
STEP 2 -> [ProductController] Import 
 [HttpGet]
    public IActionResult Import()
    {
        return View(new UploadExcelViewModel()); // Pure UI ViewModel
    }
    [HttpPost]
    public async Task<IActionResult> Import(UploadExcelViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);
        // Convert file to DTOs for service layer
        var excelStream = viewModel.ExcelFile.OpenReadStream();
        var importResult = await _importService.ImportFromExcelAsync(
            excelStream, 
            viewModel.OverwriteExisting);
        // Return results in ViewModel
        viewModel.Result = importResult;
        return View(viewModel);
    }
	[HttpPost]
public async Task<IActionResult> ImportProducts(IFormFile file)
{
    if (file == null || file.Length == 0)
        return BadRequest("File is required");

    await _productService.ImportFromExcelAsync(file);
    return RedirectToAction("Index");
}
	 ↓ (binds to)
STEP 3 -> [View] Upload.cshtml == please make it full cshtml file with css
<form asp-action="ImportProducts" method="post" enctype="multipart/form-data">
    <input type="file" name="file" accept=".xlsx" required />
    <button type="submit" class="btn btn-primary">Import</button>
</form>
@model ProductImportViewModel

<form asp-action="ImportExcel" enctype="multipart/form-data">
    <div class="form-group">
        <label>Select Excel File:</label>
        <input type="file" name="excelFile" accept=".xlsx,.xls" required />
        <small>Download template: <a href="@Url.Action("DownloadTemplate")">Template.xlsx</a></small>
    </div>
    
    <button type="submit" class="btn btn-primary">Import</button>
</form>
    ↓ (binds to)
STEP 4 -> [ViewModel] UploadExcelViewModel (UI concerns) 
public class UploadExcelViewModel
{
    [Required]
    [Display(Name = "Excel File")]
    [FileExtensions(Extensions = ".xlsx,.xls")]
    public IFormFile ExcelFile { get; set; }
    
    [Display(Name = "Overwrite existing products?")]
    public bool OverwriteExisting { get; set; }
    
    public ImportResult Result { get; set; } // Show results after import
}
public class ProductExcelUploadVM
{
    [Required]
    public IFormFile File { get; set; }
}
public class ProductImportViewModel
{
    [Required]
    [Display(Name = "Product Code")]
    public string ProductCode { get; set; }
    [Display(Name = "Category")]
    public string ProductCategory { get; set; }
    // UI-specific properties
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; }
}
public class ProductImportViewModel
{
    // Map to Excel columns
    public string ProductCode { get; set; } = string.Empty;
    public string ProductCategory { get; set; } = string.Empty; // Will parse to enum
    public string ProductType { get; set; } = string.Empty;    // Will parse to enum
    public string ColorTopSide { get; set; } = string.Empty;
    public string ColorBottomSide { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string ZincCoating { get; set; } = string.Empty;
    public string Thickness { get; set; } = string.Empty;  // String for parsing
    public string Width { get; set; } = string.Empty;      // String for parsing
    public string GrossWeight { get; set; } = string.Empty; // Fixed spelling
    public string NetWeight { get; set; } = string.Empty;   // Fixed spelling
    public string Defects { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;      // String for parsing
}
    ↓ (converts to)
STEP 5 -> [DTO] ProductExcelDto (data transfer between UI and Service)
public class ProductExcelDto
{
    // Exactly matches Excel columns
    [ExcelColumn("Product Code")]
    public string ProductCode { get; set; }
    [ExcelColumn("Category")]
    public string CategoryText { get; set; } // Raw text from Excel
    // Parse methods
    public ProductCategory ParseCategory() => 
        Enum.Parse<ProductCategory>(CategoryText);
}
public class ProductExcelRowDto
{
    public string ProductCode { get; set; }
    public string ProductCategory { get; set; }
    public decimal Price { get; set; }
}
public class ProductExcelImportDto
{
    public string ProductCode { get; set; }
    public string ProductCategory { get; set; } // string from Excel
    public string ProductType { get; set; }
    public string ColorTopSide { get; set; }
    public string ColorBottomSide { get; set; }
    public string Grade { get; set; }
    public string ZincCoating { get; set; }
    public int Thickness { get; set; }
    public int Width { get; set; }
    public int GrossWeigth { get; set; }
    public int NetWeigth { get; set; }
    public string Defects { get; set; }
    public decimal Price { get; set; }
}

    ↓ (maps to)
STEP 5 -> [Service] ProductImportService (business logic)
    public async Task<ImportResult> ImportFromExcelAsync(
        Stream excelStream, 
        bool overwriteExisting)
    {
        // 1. Parse Excel to DTOs (not ViewModels)
        var excelDtos = _excelParser.Parse<ProductExcelDto>(excelStream);
        // 2. Validate and convert to Domain Models
        var products = excelDtos.Select(dto => new Product
        {
            ProductCode = dto.ProductCode,
            ProductCategory = dto.ParseCategory(),
            // Map other properties...
        }).ToList();
        // 3. Business logic here (no UI concerns)
        if (overwriteExisting)
            await _repository.BulkUpsertAsync(products);
        else
            await _repository.BulkInsertAsync(products);
        return new ImportResult { Success = true };
    }
	public async Task ImportFromExcelAsync(IFormFile file)
{
    var rows = _excelReader.Read<ProductExcelImportDto>(file);
    foreach (var row in rows)
    {
        var product = await _productRepository
            .GetByProductCodeAsync(row.ProductCode);
        if (product == null)
        {
            product = new Product();
            _productRepository.Add(product);
        }
        product.ProductCode = row.ProductCode;
        product.ProductCategory = Enum.Parse<ProductCategory>(row.ProductCategory);
        product.ProductType = Enum.Parse<ProductType>(row.ProductType);
        product.ColorTopSide = row.ColorTopSide;
        product.ColorBottomSide = row.ColorBottomSide;
        product.Grade = row.Grade;
        product.ZincCoating = row.ZincCoating;
        product.Thickness = row.Thickness;
        product.Width = row.Width;
        product.GrossWeigth = row.GrossWeigth;
        product.NetWeigth = row.NetWeigth;
        product.Defects = row.Defects;
        product.Price = row.Price;
    }

    await _productRepository.SaveChangesAsync();
}
    ↓ (uses)
STEP 7 -> [Domain Model] Product (business entity)
 public class Product // this is fixed with requered data for auction / bidings
 {
     public int Id { get; set; }
     public string ProductCode { get; set; } = string.Empty;
     public ProductCategory ProductCategory { get; set; }
     public ProductType ProductType { get; set; }
     public string ColorTopSide { get; set; } = string.Empty;
     public string ColorBottomSide { get; set; } = string.Empty;
     public string Grade { get; set; } = string.Empty;
     public string ZincCoating { get; set; } = string.Empty;
     public decimal Thickness { get; set; }
     public int Width { get; set; }
     public decimal GrossWeight { get; set; } 
     public decimal NetWeight { get; set; }
     public string Defects { get; set; } = string.Empty;
     public decimal? Price { get; set; }
 }
    ↓ (persists via)
STEP 8 -> [Repository] ProductRepository (data access)
 public interface IRepository<T> where T : class
 {
     Task<T> GetByIdAsync(int id);
     Task<IEnumerable<T>> GetAllAsync();
     Task AddAsync(T entity);
     Task UpdateAsync(T entity);
     Task DeleteAsync(int id);
     Task<bool> ExistsAsync(int id);
 }
 public interface IProductRepository : IRepository<Product>
 {
     Task<Product> GetByProductCodeAsync(string productCode);
     Task<bool> ProductCodeExistsAsync(string productCode);
     Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category);
     Task BulkInsertOrUpdateAsync(IEnumerable<Product> products);
     Task<int> GetCountAsync();
     Task SaveChangesAsync();
 }

