 using System.Data.SqlClient;

 class SalesApplication
 {
     private string connectionString;

     public SalesApplication(string connectionString)
     {
         this.connectionString = connectionString;
     }

     public void Run()
     {
         using (SqlConnection connection = new SqlConnection(connectionString))
         {
             connection.Open();

             CreateTable(connection, "Customers",
                 "CREATE TABLE Customers (Id INT PRIMARY KEY IDENTITY, Name VARCHAR(100))");
             CreateTable(connection, "Articles",
                 "CREATE TABLE Articles (Id INT PRIMARY KEY IDENTITY, Name VARCHAR(100), Price DECIMAL(10, 2))");
             CreateTable(connection, "Invoices",
                 "CREATE TABLE Invoices (Id INT PRIMARY KEY IDENTITY, CustomerId INT, FOREIGN KEY (CustomerId) REFERENCES Customers(Id))");

             bool exit = false;
             while (!exit)
             {
                 Console.WriteLine("Sales Application Menu:");
                 Console.WriteLine("1. Create Customer");
                 Console.WriteLine("2. Create Article");
                 Console.WriteLine("3. Create Invoice");
                 Console.WriteLine("4. Exit");

                 Console.Write("Enter your choice: ");
                 string choice = Console.ReadLine();

                 switch (choice)
                 {
                     case "1":
                         CreateCustomer(connection);
                         break;
                     case "2":
                         CreateArticle(connection);
                         break;
                     case "3":
                         CreateInvoice(connection);
                         break;
                     case "4":
                         exit = true;
                         break;
                     default:
                         Console.WriteLine("Invalid choice. Please try again.");
                         break;
                 }
             }
         }
     }

     static void CreateTable(SqlConnection connection, string tableName, string createQuery)
     {
         string checkTableExistsQuery =
             $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
         using (SqlCommand command = new SqlCommand(checkTableExistsQuery, connection))
         {
             int tableCount = Convert.ToInt32(command.ExecuteScalar());
             if (tableCount == 0)
             {
                 using (SqlCommand createCommand = new SqlCommand(createQuery, connection))
                 {
                     createCommand.ExecuteNonQuery();
                     Console.WriteLine($"{tableName} table created.");
                 }
             }
         }
     }

     static void CreateCustomer(SqlConnection connection)
     {
         Console.Write("Enter customer name: ");
         string name = Console.ReadLine();

         string insertQuery = "INSERT INTO Customers (Name) VALUES (@Name)";
         using (SqlCommand command = new SqlCommand(insertQuery, connection))
         {
             command.Parameters.AddWithValue("@Name", name);
             command.ExecuteNonQuery();
             Console.WriteLine("Customer created successfully.");
         }
     }

     static void CreateArticle(SqlConnection connection)
     {
         Console.Write("Enter article name: ");
         string name = Console.ReadLine();

         Console.Write("Enter article price: ");
         decimal price = Convert.ToDecimal(Console.ReadLine());

         string insertQuery = "INSERT INTO Articles (Name, Price) VALUES (@Name, @Price)";
         using (SqlCommand command = new SqlCommand(insertQuery, connection))
         {
             command.Parameters.AddWithValue("@Name", name);
             command.Parameters.AddWithValue("@Price", price);
             command.ExecuteNonQuery();
             Console.WriteLine("Article created successfully.");
         }
     }

     static void CreateInvoice(SqlConnection connection)
     {
         Console.Write("Enter customer ID: ");
         int customerId = Convert.ToInt32(Console.ReadLine());

         Console.Write("Enter article ID:");
         int articleId = Convert.ToInt32(Console.ReadLine());

         string insertQuery = "INSERT INTO Invoices (CustomerId) VALUES (@CustomerId); SELECT SCOPE_IDENTITY();";
         using (SqlCommand command = new SqlCommand(insertQuery, connection))
         {
             command.Parameters.AddWithValue("@CustomerId", customerId);
             int invoiceId = Convert.ToInt32(command.ExecuteScalar());

             string insertInvoiceArticleQuery =
                 "INSERT INTO InvoiceArticles (InvoiceId, ArticleId) VALUES (@InvoiceId, @ArticleId)";
             using (SqlCommand invoiceArticleCommand = new SqlCommand(insertInvoiceArticleQuery, connection))
             {
                 invoiceArticleCommand.Parameters.AddWithValue("@InvoiceId", invoiceId);
                 invoiceArticleCommand.Parameters.AddWithValue("@ArticleId", articleId);
                 invoiceArticleCommand.ExecuteNonQuery();
             }

             Console.WriteLine("Invoice created successfully.");
         }
     }
 }