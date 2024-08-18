using currentworkingsassyplanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace currentworkingsassyplanner.Pages
{
    // Made by Adam Sinclair, edited by Sam Duggan
    public class ShopModel : PageModel
    {
        private readonly currentworkingsassyplanner.Data.SPContext _context;
        public ShopModel(currentworkingsassyplanner.Data.SPContext context)
        {
            _context = context;
        }

        public enum ProductType
        {
            AcademicPlanner, WeeklyLessonPlanner, DailyLessonPlanner, Notebook, Sticker
        }

        public enum Style
        {
            Floral, Animals, Pattern, Landscape, Other
        }

        //Sam's code start
        [BindProperty]
        //Sam's code end
        public List<List<Product>> CategorisedProductTBL { get; set; } = new List<List<Product>>();

        [BindProperty]
        public List<List<Product>> StyleProductTBL { get; set; } = new List<List<Product>>();

        //Sam's code start
        [BindProperty(Name = "FilteredCategories")]
        public string[] FilteredCategories { get; set; }

        [BindProperty(Name = "FilteredStyles")]
        public string[] FilteredStyles { get; set; }


        public bool isFiltered = false;
        //Sam's code end

        public void OnGet()
        {
            
            GetAllProducts();
        }

        public string getTitle(string item)
        {
            string title = "";
            switch (item)
            {
                case "AcademicPlanner":
                    title = "Academic Planners";
                    break;
                case "WeeklyLessonPlanner":
                    title = "Weekly Lesson Planners";
                    break;
                case "DailyLessonPlanner":
                    title = "Daily Lesson Planners";
                    break;
                case "Notebook":
                    title = "Notebooks";
                    break;
                case "Sticker":
                    title = "Stickers";
                    break;
            }
            return title;
        }

        private void GetAllProducts()
        {
            List<List<Product>> categorisedProducts = new List<List<Product>>();
            //List<List<Product>> productsbyStyle = new List<List<Product>>();


            for (int i = 0; i < Enum.GetValues(typeof(ProductType)).Length; i++)
            {
                var productTypeEnums = Enum.GetValues(typeof(ProductType));
                var currentEnum = productTypeEnums.GetValue(i).ToString();
                categorisedProducts.Add(_context.Products.FromSqlRaw("SELECT * FROM Product WHERE ProductType= {0}", currentEnum).ToList());

            }

            List<List<Product>> productsByStyle = new List<List<Product>>();

            for (int i = 0; i < Enum.GetValues(typeof(Style)).Length; i++)
            {
                var styleEnums = Enum.GetValues(typeof(Style));
                var currentEnum = styleEnums.GetValue(i).ToString();
                productsByStyle.Add(_context.Products.FromSqlRaw("SELECT * FROM Product WHERE Style= {0}", currentEnum).ToList());

            }

            CategorisedProductTBL = categorisedProducts;
            StyleProductTBL = productsByStyle;

        }

        /*static void RemoveDuplicates(List<List<Product>> list1, List<List<Product>> list2)
        {
            HashSet<Product> uniqueProducts = new HashSet<Product>();

            // Remove duplicates from list1
            foreach (List<Product> productList in list1)
            {
                foreach (Product product in productList)
                {
                    if (!uniqueProducts.Contains(product))
                    {
                        uniqueProducts.Add(product);
                    }
                }
            }

            // Remove duplicates from list2 and add unique products to uniqueProducts HashSet
            foreach (List<Product> productList in list2)
            {
                foreach (Product product in productList)
                {
                    if (!uniqueProducts.Contains(product))
                    {
                        uniqueProducts.Add(product);
                    }
                }
            }

            // Clear list1 and list2 and add unique products back to them
            list1.Clear();
            list2.Clear();

            foreach (Product product in uniqueProducts)
            {
                list1.Add(new List<Product> { product });
                list2.Add(new List<Product> { product });
            }
        }*/


        public IActionResult OnPostApplyFilter()
        {
            isFiltered = true;
            GetAllProducts();

            string theQuery = string.Empty;

            //If the user wants to filter by category
            if(FilteredCategories.Length > 0)
            {
                //Initial SQL
                theQuery += "SELECT * FROM Product WHERE (ProductType = ";

                //If the user only wants to filter by one category
                if (FilteredCategories.Length == 1)
                {
                    theQuery += $"'{FilteredCategories[0]}' ";
                }
                //If the user wants to filter by multiple
                else if (FilteredCategories.Length > 1)
                {
                    for (int i = 0; i < FilteredCategories.Length; i++)
                    {
                        if (i != FilteredCategories.Length - 1)
                        {
                            theQuery += $"'{FilteredCategories[i]}' OR ProductType = ";

                        }
                        else
                        {
                            theQuery += $"'{FilteredCategories[i]}'";
                        }

                    }
                }


                theQuery += ") ";

                //If the user also wants to filter on style
                if (FilteredStyles.Length > 0)
                {
                    theQuery += " AND (Style = ";

                    if (FilteredStyles.Length == 1)
                    {
                        theQuery += $"'{FilteredStyles[0]}' ";
                    }
                    else if (FilteredStyles.Length > 1)
                    {
                        for (int i = 0; i < FilteredStyles.Length; i++)
                        {
                            if (i != FilteredStyles.Length - 1)
                            {
                                theQuery += $"'{FilteredStyles[i]}' OR Style = ";

                            }
                            else
                            {
                                theQuery += $"'{FilteredStyles[i]}'";
                            }

                        }
                    }

                    theQuery += ")";
                }

            }
            //If the user does not want to filter on category, but on style
            else if(FilteredStyles.Length > 0)
            {
                //Initial SQL query
                theQuery += "SELECT * FROM Product WHERE (Style = ";

                //If the user only wants to filter on one style
                if (FilteredStyles.Length == 1)
                {
                    theQuery += $"'{FilteredStyles[0]}' ";
                }
                //If the user wants to filter on multiple styles
                else if (FilteredStyles.Length > 1)
                {
                    for (int i = 0; i < FilteredStyles.Length; i++)
                    {
                        if (i != FilteredStyles.Length - 1)
                        {
                            theQuery += $"'{FilteredStyles[i]}' OR Style = ";

                        }
                        else
                        {
                            theQuery += $"'{FilteredStyles[i]}'";
                        }

                    }
                }

                theQuery += ")";
            }

            //If the user has pressed apply filter without selecting any, don't do anyhting
            if(theQuery != string.Empty)
            {
                CategorisedProductTBL = new List<List<Product>>();
                CategorisedProductTBL.Add(_context.Products.FromSqlRaw(theQuery).ToList());
            }
            
            return Page();

            /*string mainSqlQuery = "SELECT * FROM Product WHERE (ProductType = ";

            if (FilteredCategories.Length == 1)
            {
                mainSqlQuery += $"'{FilteredCategories[0]}' ";
            }
            else if (FilteredCategories.Length > 1)
            {
                for (int i = 0; i < FilteredCategories.Length; i++)
                {
                    if (i != FilteredCategories.Length - 1)
                    {
                        mainSqlQuery += $"'{FilteredCategories[i]}' OR ProductType = ";

                    }
                    else
                    {
                        mainSqlQuery += $"'{FilteredCategories[i]}'";
                    }

                }
            }

            

            mainSqlQuery += ") ";



            if (FilteredStyles.Length > 0)
            {
                mainSqlQuery += " AND (Style = ";

                if (FilteredStyles.Length == 1)
                {
                    mainSqlQuery += $"'{FilteredStyles[0]}' ";
                }
                else if (FilteredStyles.Length > 1)
                {
                    for (int i = 0; i < FilteredStyles.Length; i++)
                    {
                        if (i != FilteredStyles.Length - 1)
                        {
                            mainSqlQuery += $"'{FilteredStyles[i]}' OR Style = ";

                        }
                        else
                        {
                            mainSqlQuery += $"'{FilteredStyles[i]}'";
                        }

                    }
                }

                mainSqlQuery += ")";
            }*/

            /*CategorisedProductTBL = new List<List<Product>>();
            CategorisedProductTBL.Add(_context.Products.FromSqlRaw(mainSqlQuery).ToList());
            return Page();*/
        }

        /*public IActionResult OnPostApplyFilter()
        {
            //PopulateCategorisedProductTBL(); // Re-populate CategorisedProductTBL (posting empties it)
            GetAllProducts(); // <-- To replace PopulateCategorisedProductTBL()
            List<List<Product>> tempCategorisedProductTBL = new List<List<Product>>(); //create empty list
            List<List<Product>> tempstyles = new List<List<Product>>();
            if (FilteredCategories != null && FilteredCategories.Length > 0) //only do if filters applied
            {
                foreach (var filter in FilteredCategories) //iterate through all applied filters
                {                                          //add all filtered products to new list

                    switch (filter)
                    {
                        case "AcademicPlanner":
                            tempCategorisedProductTBL.Add(CategorisedProductTBL[0]);
                            break;
                        case "WeeklyLessonPlanner":
                            tempCategorisedProductTBL.Add(CategorisedProductTBL[1]);
                            break;
                        case "DailyLessonPlanner":
                            tempCategorisedProductTBL.Add(CategorisedProductTBL[2]);
                            break;
                        case "Notebook":
                            tempCategorisedProductTBL.Add(CategorisedProductTBL[3]);
                            break;
                        case "Sticker":
                            tempCategorisedProductTBL.Add(CategorisedProductTBL[4]);
                            break;
                    }

                }
                
                
            }
            else
            {
                //If no filters were selected and the apply button was still pressed, load everything
                //PopulateCategorisedProductTBL();
                GetAllProducts(); // <-- To replace PopulateCategorisedProductTBL()
            }
            if (FilteredStyles != null && FilteredStyles.Length > 0) //only do if filters applied
            {
                
                foreach (var filter in FilteredStyles) //iterate through all applied filters
                {                                          //add all filtered products to new list

                    switch (filter)
                    {
                        case "Floral":
                            tempstyles.Add(StyleProductTBL[0]);
                            break;
                        case "Animals":
                            tempstyles.Add(StyleProductTBL[1]);
                            break;
                        case "Pattern":
                            tempstyles.Add(StyleProductTBL[2]);
                            break;
                        case "Landscape":
                            tempstyles.Add(StyleProductTBL[3]);
                            break;
                        case "Other":
                            tempstyles.Add(StyleProductTBL[4]);
                            break;
                    }

                }
            }
            else
            {
                //If no filters were selected and the apply button was still pressed, load everything
                //PopulateCategorisedProductTBL();
                GetAllProducts(); // <-- To replace PopulateCategorisedProductTBL()
            }
            

            *//*foreach (var filter in FilteredCategories) 
            { 
                for(int i = 0; i < tempstyles.Count; i++)
                {
                    for(int j = 0; j < tempstyles[i].Count; j++)
                    {

                    }
                }
            }*//*

            List<Product> randomlist = new List<Product>();
            for(int i = 0; i < tempstyles.Count; i++)
            {
                var styleList = tempstyles[i];
                foreach(var product in styleList)
                {
                    foreach(var filter in FilteredCategories)
                    {
                        if(product.ProductType == filter)
                        {
                            randomlist.Add(product);
                        }
                    }
                    
                }
            }

            tempCategorisedProductTBL.Add(randomlist);

            //RemoveDuplicates(tempCategorisedProductTBL, tempstyles);

            CategorisedProductTBL = tempCategorisedProductTBL; //set list of products to new list
            return Page(); //reload page
        }*/

        /*

        public List<List<Product>> filterStyle(int index, List<List<Product>> tempCategorisedProductTBL)
        {
            if (FilteredStyles != null && FilteredStyles.Length > 0) //only do if filters applied
            {
                for (var i = 0; i < CategorisedProductTBL[index].Count(); i++) //iterate through all applied filters
                {
                    foreach (var styFilter in FilteredStyles)
                    {
                        switch (styFilter)
                        {
                            case "floral":
                                tempCategorisedProductTBL[index].Add(StyleProductTBL[0][i]);
                                break;
                            case "animals":
                                tempCategorisedProductTBL[index].Add(StyleProductTBL[1][i]);
                                break;
                            case "pattern":
                                tempCategorisedProductTBL[index].Add(StyleProductTBL[2][i]);
                                break;
                            case "landscape":
                                tempCategorisedProductTBL[index].Add(StyleProductTBL[3][i]);
                                break;
                            case "other":
                                tempCategorisedProductTBL[index].Add(StyleProductTBL[4][i]);
                                break;

                        }

                    }
                }
                
            }
            else
            {
                tempCategorisedProductTBL.Add(CategorisedProductTBL[index]);
            }

            return tempCategorisedProductTBL;

        }

        //Sam's code start
        public IActionResult OnPostApplyFilter()
        {
            if (FilteredCategories != null && FilteredCategories.Length > 0) //only do if filters applied
            {
                
                GetAllProducts(); 

                List<List<Product>> tempCategorisedProductTBL = new List<List<Product>>(); //create empty list
                foreach (var catFilter in FilteredCategories) //iterate through all applied filters
                {                                          //add all filtered products to new list

                    switch (catFilter) //look at all categories selected
                    {
                        case "academicPlanner": 
                            tempCategorisedProductTBL = filterStyle(0, tempCategorisedProductTBL);
                            break;
                        case "weeklyPlanner":
                            tempCategorisedProductTBL = filterStyle(1, tempCategorisedProductTBL);
                            break;
                        case "dailyPlanner":
                            tempCategorisedProductTBL = filterStyle(2, tempCategorisedProductTBL);
                            break;
                        case "notebook":
                            tempCategorisedProductTBL = filterStyle(3, tempCategorisedProductTBL);
                            break;
                        case "sticker":
                            tempCategorisedProductTBL = filterStyle(4, tempCategorisedProductTBL);
                            break;

                    }

                }
                CategorisedProductTBL = tempCategorisedProductTBL; //set list of products to new list
            }
            else
            {
                //If no filters were selected and the apply button was still pressed, load everything
                //PopulateCategorisedProductTBL();
                GetAllProducts(); // <-- To replace PopulateCategorisedProductTBL()
            }

            return Page(); //reload page
        }
        //Sam's code end 
        */
    }
}