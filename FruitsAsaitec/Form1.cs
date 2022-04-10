using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FruitsAsaitec
{
    public partial class fruitShop : Form
    {
        public fruitShop()
        {
            InitializeComponent();
        }

        private void calculate_Click(object sender, EventArgs e)
        {
            //Declarations
            List<productPrice> ProductPrice = new List<productPrice>();
            List<purchase> Purchase = new List<purchase>();

            string[] linesAux;

            using (var writer = new System.IO.StreamWriter(Application.StartupPath + "\\Out.txt"))
            {
                //Console.SetOut(writer);
            }

                

            try
            {
                //File D. configuration
                openFileDialog.Multiselect = false;
                openFileDialog.InitialDirectory = Application.ExecutablePath;

                openFileDialog.Title = "[step 1/2] Select the price file (Product,Price)";
                openFileDialog.ShowDialog(this);
                //Process Product file
                if (openFileDialog.FileName != "")
                {
                    linesAux = System.IO.File.ReadAllLines(openFileDialog.FileName);
                    foreach (string x in linesAux.SkipWhile(b => b.StartsWith("PRODUCT")))
                    {
                        ProductPrice.Add(new productPrice
                        {
                            product = x.Split(',')[0],
                            price = decimal.Parse(x.Split(',')[1].Replace("€", ""))
                        });
                    }
                }
                else
                {
                    throw new Exception("File not selected");
                }

                openFileDialog.Title = "[step 2/2] Select the purchase file (Product,Quantity)";
                openFileDialog.ShowDialog(this);

                //Process Purchase file
                if (openFileDialog.FileName != "")
                {
                    linesAux = System.IO.File.ReadAllLines(openFileDialog.FileName);

                    foreach (string x in linesAux.SkipWhile(b=> b.StartsWith("PRODUCT") ))
                    {
                        Purchase.Add(new purchase
                        {
                            item = x.Split(',')[0],
                            quantity = decimal.Parse(x.Split(',')[1].Replace("€", ""))
                        });
                    }

                }
                else
                {
                    throw new Exception("File not selected");
                }

                List<offers> offer = new List<offers> { new offers {product= "Pear", gift= "", quantity= 0, amountToDiscount= 1, minimunQuantity= 4, } };

                Calculate(ProductPrice, Purchase,offer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There are problems with the files, please verify the files and format files \r\n" + ex.Message );
            }
            Console.OpenStandardOutput();

        }

        void Calculate(List<productPrice> product, List<purchase> purchased, List<offers> offer)
        {
            //Group products
            List<purchase> purchaseGroup = purchased.GroupBy(c => c.item).Select(g => new purchase { item = g.Key, quantity = g.Sum(a => a.quantity) }).ToList();

            foreach (var item in purchaseGroup)
            {
                Console.WriteLine(item.item + "\t" + item.quantity.ToString());
                System.IO.File.WriteAllText(Application.StartupPath + "\\OUT.txt", item.item + "\t" + item.quantity.ToString());
                receipt.Items.Add(item.item + "\t" + item.quantity.ToString());
            }

            //DISCOUNTS
            foreach (var item in purchaseGroup)
            {
                offers discount = offer.Find(c => c.product == item.item);
                if (discount!= null)
                {
                    if (purchaseGroup.Find(x => x.item==discount.gift ).item !=null )
                    {
                        Console.WriteLine("DISCOUNT ***(Pending of completion)");
                        System.IO.File.AppendAllText(Application.StartupPath + "\\OUT.txt", "DISCOUNT ***(Pending of completion)");
                        receipt.Items.Add("DISCOUNT ***(Pending of completion)");

                    }
                }
            }

            //CALCULATE
            Console.WriteLine("ITEMS \t AMOUNT ");
            foreach (var item in purchaseGroup)
            {
                string txt = item.item + "\t" + (product.Find(c => c.product == item.item).price * item.quantity).ToString() + " €";
                Console.WriteLine(txt);
                System.IO.File.AppendAllText(Application.StartupPath + "\\OUT.txt", txt);
                receipt.Items.Add(txt);

            }

            Console.WriteLine("TOTAL: ");
            Console.WriteLine(purchaseGroup.Select(x => product.Find(c => c.product == x.item).price * x.quantity).Sum().ToString() + " €");
            System.IO.File.AppendAllText(Application.StartupPath + "\\OUT.txt", purchaseGroup.Select(x => product.Find(c => c.product == x.item).price * x.quantity).Sum().ToString() + " €");
            receipt.Items.Add("TOTAL");
            System.IO.File.AppendAllText(Application.StartupPath + "\\OUT.txt", purchaseGroup.Select(x => product.Find(c => c.product == x.item).price * x.quantity).Sum().ToString() + " €");
            receipt.Items.Add(purchaseGroup.Select(x => product.Find(c => c.product == x.item).price * x.quantity).Sum().ToString() + " €");






        }
    }

    class offers{
        public string product;
        public string gift;
        public decimal quantity;
        public decimal amountToDiscount=0;
        public decimal minimunQuantity = 0;

    }


    class productPrice
    {
        public string product;
        public decimal price;
    }

    class purchase
    {
        public string item;
        public decimal quantity;
    }
}


