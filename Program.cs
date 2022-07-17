using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapGet("/header-response", (HttpResponse response) => {
    response.Headers.Add("Nome", "Alberto Benites");
    return new {tipo = "header",  name = "response"};
});

app.MapGet("/date-product", ([FromQuery] string dateStart, [FromQuery] string dateEnd) => {
    return dateStart + " - data request- " + dateEnd;
});

app.MapGet("/one-product/{id}", ([FromRoute] string id) => {
    return id;
});

app.MapGet("/header-request", (HttpRequest request) => {
    return "id do produto: " + request.Headers["product-id"].ToString() + " recebido";
});

app.MapPost("/product", (Product product) => {
    ProductRepository.Add(product);
    return Results.Created("/prodcut/" + product.Id, product.Id + " - " + product.Name);
    });

app.MapGet("/product/{id}", ([FromRoute] string id) => {
    var product = ProductRepository.GetBy(id);
    if(product != null)
        return Results.Ok(product);
    return Results.NotFound();
});

app.MapGet("/products", () => {
    var listProducts = ProductRepository.ListProducts();
    return Results.Ok(listProducts);
});

app.MapPut("/product", (Product product) => {
    var productSearched = ProductRepository.GetBy(product.Id);
    productSearched.Name = product.Name;
    Results.Ok();
});

app.MapDelete("/product/{id}", ([FromRoute] string id) => {
    var productSearched = ProductRepository.GetBy(id);
    ProductRepository.Remove(productSearched);
    Results.Ok();
});
if(app.Environment.IsDevelopment())
app.MapGet("/configuration/database" , (IConfiguration configuration ) => {
    return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
});

app.Run();

public static class ProductRepository{
    public static List<Product> Products { get; set; } =  Products = new List<Product>();

    public static void Init(IConfiguration configuration){
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;

    }
    public static void Add(Product product) {
        Products.Add(product);
    }
    public static Product GetBy(string id) {
        return Products.FirstOrDefault(p => p.Id == id);
    }

    public static void Remove(Product product) {
        Products.Remove(product);
    }

    public static List<Product> ListProducts(){
        return ProductRepository.Products;
    }
}

public class Product {
public string Id { get; set; }
public string Name { get; set; }
}