using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Store.DAL;
using Store.Models;

namespace Store.Controllers
{
    public class ProductController : Controller
    {
        private StoreContext db = new StoreContext();

        // GET: Product
        public ActionResult Index()
        {
            var viewModel = from p in db.Products
                            from i in db.Images.Where(o => p.ProductId == o.ProductId)
                            .Take(1)
                            .DefaultIfEmpty()
                            select new ProductListVM
                            {
                                ProductId = p.ProductId,
                                Images = i,
                                ProductName = p.ProductName,
                                ProductSummary = p.ProductSummary,
                                Price = p.Price,
                                Quantity = p.Quantity
                            };


            return View(viewModel.ToList());

        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            List<Image> images = db.Images.Where(i => i.ProductId == product.ProductId).ToList();
            var productDetail = new ProductDetailVM
            {
                ProductId = product.ProductId,
                Images = images,
                ProductName = product.ProductName,
                ProductSummary = product.ProductSummary,
                ProductSpecification = product.ProductSpecification,
                Price = product.Price,
                Quantity = product.Quantity
            };

            return View(productDetail);
        }

        public ActionResult AddToCard(ProductListVM product)
        {
            List<Card> cardItems = new List<Card>();
            if (Session["Card"] != null)
                cardItems = (List<Card>)Session["Card"];
            if (cardItems.Any(c => c.ProductId == product.ProductId))
            {
                Card item = cardItems.Where(c => c.ProductId == product.ProductId).SingleOrDefault();
                cardItems.Remove(item);
                item.Count++;
                item.Sum = item.Count * item.Price;
                cardItems.Add(item);
            }
            else
            {
                cardItems.Add(new Card
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Count = 1,
                    Sum = product.Price
                });
            }

            Session["Card"] = cardItems;
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromCard(Card card)
        {
            List<Card> cardItems = new List<Card>();
            if (Session["Card"] != null)
                cardItems = (List<Card>)Session["Card"];
            if (cardItems.Any(c => c.ProductId == card.ProductId))
            {
                Card item = cardItems.Where(c => c.ProductId == card.ProductId).SingleOrDefault();
                cardItems.Remove(item);
            }

            Session["Card"] = cardItems;
            return RedirectToAction("Card");
        }

        public ActionResult Card()
        {
            List<Card> cardItems = (List<Card>)Session["Card"];
            return View(cardItems);
        }

        [HttpPost]
        public ActionResult Checkout()
        {
            List<Card> cardItems = (List<Card>)Session["Card"];
            foreach(Card cardItem in cardItems)
            {
                var product = db.Products.Find(cardItem.ProductId);
                product.Quantity = product.Quantity - cardItem.Count;
                db.SaveChanges();
            }
            Session.Clear();
            return RedirectToAction("Index", new { purchased = 1 });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
