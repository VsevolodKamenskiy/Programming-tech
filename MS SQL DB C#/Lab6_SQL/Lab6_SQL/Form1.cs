using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace Lab6_SQL
{
    
    
    public partial class Form1 : Form
    {

        SqlConnection cnn = new SqlConnection();
        
                                       
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)//Событие загрузки формы
        {

            SqlConnectionStringBuilder bdr = new SqlConnectionStringBuilder();//создаём конструктор строки подключения
            bdr.DataSource = "localhost\\SQLEXPRESS";
            bdr.InitialCatalog = "FastFood";
            bdr.IntegratedSecurity = true;
            cnn.ConnectionString = bdr.ConnectionString;//создали и присвоили строку подключения
            
            
                try
                {

                cnn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandText = "SELECT * FROM Chain_Cafes";//прописываем sql запрос

                //ЗАПОЛНЕНИЕ таблицы
                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter();
                ad.SelectCommand = cmd;
                ad.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.Columns[0].Visible = false;//убрать стобец ID
                dataGridView1.ReadOnly = true;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;//чтобы можно было выбирать строку целиком
                dataGridView1.MultiSelect = false;

                for (Int16 i = 1; i < dataGridView1.ColumnCount; i++)//заполняем combobox1,3 заголовками столбцов из таблицы
                {
                    comboBox1.Items.Add(dataGridView1.Columns[i].HeaderCell.Value);
                    comboBox3.Items.Add(dataGridView1.Columns[i].HeaderCell.Value);
                }

                cnn.Close();

            }
            catch(Exception exc)
            {
                MessageBox.Show("Unable connect to the database!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            
        }

        private void button1_Click(object sender, EventArgs e)//удаление
        {

            try
            {
                cnn.Open();
               // cnn.Close();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandText = "DELETE FROM Chain_Cafes Where Chain_Cafes.ID = " + dataGridView1.CurrentRow.Cells[0].Value.ToString();
                //MessageBox.Show(cmd.CommandText);
                cmd.ExecuteNonQuery();//выполнить запрос в бд

                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);//удаление строки из табл в с#*/

                for (int i = 0; i < dataGridView1.RowCount; ++i)//цикл для пересчета индексов ID записи, нужно для того, чтобы id-ШНИКИ ПОСЛЕ удаления упорядочились
                {

                    cmd.CommandText = "UPDATE Chain_Cafes SET ID = " + i.ToString() + " WHERE ID = " + dataGridView1.Rows[i].Cells[0].Value.ToString();
                    cmd.ExecuteNonQuery();

                }

                
            }
            catch (Exception exc)
            {
                MessageBox.Show("Unable to delete record", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);//ошибка удления записи
            }

            cnn.Close();//закрыть подключение
        }

        private void button2_Click(object sender, EventArgs e)//добавление
        {
                  
             try//пробуем добавить запись
             {
                cnn.Open();
                Int32 i = dataGridView1.RowCount;//число строк в таблице, для того чтобы ID новой записи был корректен

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;

                cmd.CommandText = "INSERT INTO Chain_Cafes (ID,Name,Amount,Date,Invoice,Vegetarian,Wi_Fi,Time,Salary) VALUES(" +
                i.ToString() + ",'" + textBox9.Text + "'," + textBox10.Text + ",'" + maskedTextBox1.Text + "'," + textBox11.Text + "," +
                Convert.ToInt16(checkBox1.Checked).ToString() + "," + Convert.ToInt16(checkBox2.Checked).ToString() + ",'" + maskedTextBox2.Text + 
                "'," + textBox8.Text + ")";//формируем sql запрос
                //MessageBox.Show(cmd.CommandText);

                cmd.ExecuteNonQuery();//выполнение запроса

                //перерисовка таблицы
                cmd.CommandText = "SELECT * FROM Chain_Cafes";
                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter();
                ad.SelectCommand = cmd;
                ad.Fill(dt);
                dataGridView1.DataSource = dt;
                
            }
            catch (Exception exc)
            {
                MessageBox.Show("Invalid input","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            cnn.Close();

        }

        
        private void button3_Click(object sender, EventArgs e)//базовая фуеция изменения записи
        {                               

                try//пробуем изменить запись
                {
                    cnn.Open();
                    Int32 i = dataGridView1.CurrentRow.Index;//получаем индекс выбранной строки

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cnn;

                    cmd.CommandText = "UPDATE Chain_Cafes SET Name = '" + textBox9.Text + "', Amount = " + textBox10.Text + ", Date = '" +
                    maskedTextBox1.Text + "', Invoice = " + textBox11.Text + ", Vegetarian = " + Convert.ToInt16(checkBox1.Checked).ToString() +
                    ", Wi_Fi = " + Convert.ToInt16(checkBox2.Checked).ToString() + ", Time = '" + maskedTextBox2.Text + "', Salary = " +
                    textBox8.Text + " WHERE ID = " + dataGridView1.Rows[i].Cells[0].Value.ToString();//SQL запрос на изменение записи в бд
                    //MessageBox.Show(cmd.CommandText);
                    cmd.ExecuteNonQuery();


                    //перерисовка таблицы
                    cmd.CommandText = "SELECT * FROM Chain_Cafes";
                    DataTable dt = new DataTable();
                    SqlDataAdapter ad = new SqlDataAdapter();
                    ad.SelectCommand = cmd;
                    ad.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.Rows[i].Selected = true;//возвращаем фокус на строку
                    //перерисовка таблицы

                }
                catch (Exception exc)
                {
                    MessageBox.Show("Invalid input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);//сообщение о некорректности ввода данных
                }
              
            cnn.Close();

        }

        

        private void button4_Click(object sender, EventArgs e)//считаем маскимальное минимальное среднее по столбцам
        {
            cnn.Open();//открытие подключения
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandText = "SELECT * FROM Chain_Cafes";
            DataTable dt = new DataTable();//набор данных 
            SqlDataAdapter ad = new SqlDataAdapter();//адаптер для набора данных
            ad.SelectCommand = cmd;
            ad.Fill(dt);

            //вычисление занчений через встрoенные функции SQL
            Int32 maxSalary = Convert.ToInt32(dt.Compute("max([Salary])", string.Empty));
            Int32 minSalary = Convert.ToInt32(dt.Compute("min([Salary])", string.Empty));
            Int32 avSalary = Convert.ToInt32(dt.Compute("avg([Salary])", string.Empty));

            Int32 maxAmount = Convert.ToInt32(dt.Compute("max([Amount])", string.Empty));
            Int32 minAmount = Convert.ToInt32(dt.Compute("min([Amount])", string.Empty));
            Int32 avAmount = Convert.ToInt32(dt.Compute("avg([Amount])", string.Empty));

            Int32 maxInvoice = Convert.ToInt32(dt.Compute("max([Invoice])", string.Empty));
            Int32 minInvoice = Convert.ToInt32(dt.Compute("min([Invoice])", string.Empty));
            Int32 avInvoice = Convert.ToInt32(dt.Compute("avg([Invoice])", string.Empty));

            MessageBox.Show("The maximal salary is: " + maxSalary.ToString() + "\n" +
                            "The minimal salary is: " + minSalary.ToString() + "\n" +
                            "The average salary is: " + avSalary.ToString() + "\n\n" +
                            "The maximal amount is: " + maxAmount.ToString() + "\n" +
                            "The minimal amount is: " + minAmount.ToString() + "\n" +
                            "The average amount is: " + avAmount.ToString() + "\n\n" +
                            "The maximal invoice is: " + maxInvoice.ToString() + "\n" +
                            "The minimal invoice is: " + minInvoice.ToString() + "\n" +
                            "The average invoice is: " + avInvoice.ToString() + "\n", "Calculations",
                            MessageBoxButtons.OK, MessageBoxIcon.Information );
            cnn.Close();
        }

        
        private void textBox1_TextChanged(object sender, EventArgs e)// вывод строк содержащих заданное значение
        {

            try
            {
                cnn.Open();//открываем подключение к бд
                //перерисовка таблицы
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT * FROM Chain_Cafes WHERE " + comboBox1.Text + " LIKE '%" + textBox1.Text + "%'";//запрос на выборку строк
                //MessageBox.Show(cmd.CommandText);
                cmd.Connection = cnn;
                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter();
                ad.SelectCommand = cmd;
                ad.Fill(dt);
                dataGridView1.DataSource = dt;
                
            }
            catch(Exception exc)
            {
                MessageBox.Show("Invalid input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            cnn.Close();//закрываем подключение к бд

        }

        private void button5_Click(object sender, EventArgs e)//подсчёт количества записей, удовл условию
        {
            MessageBox.Show("The number of records for this condition is: " + dataGridView1.RowCount.ToString(), "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

       
        private void exitAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button6_Click(object sender, EventArgs e)//выборка по значениям
        {
            try
            {
                cnn.Open();//открываем подключение к бд
                //перерисовка таблицы
                SqlCommand cmd = new SqlCommand();

                if (comboBox2.Text == "Date")//ЕСЛИ НУЖНА ВЫБОРКА ПО ДАТЕ, ТАК КАК ФОРМАТ SQL-ЗАПРОСА ДРУГОЙ!!!
                {
                    cmd.CommandText = "SELECT * FROM Chain_Cafes WHERE Date >= '" + maskedTextBox3.Text + "' AND Date <= '" + maskedTextBox4.Text + "'";//запрос на выборку строк
                }
                else//выборка по остальным 
                {
                    cmd.CommandText = "SELECT * FROM Chain_Cafes WHERE " + comboBox2.Text + " BETWEEN " + maskedTextBox3.Text + " AND " + maskedTextBox4.Text;//запрос на выборку строк
                }
                //MessageBox.Show(cmd.CommandText);
                cmd.Connection = cnn;
                DataTable dt = new DataTable();//используется для заполнения датагридвью
                SqlDataAdapter ad = new SqlDataAdapter();//используется для заполнения датагридвью
                ad.SelectCommand = cmd;
                ad.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            cnn.Close();//закрываем подключение к бд

        }

        private void button7_Click(object sender, EventArgs e)//обновить форму
        {
   

            cnn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT * FROM Chain_Cafes";
            cmd.Connection = cnn;
            DataTable dt = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            ad.SelectCommand = cmd;
            ad.Fill(dt);
            dataGridView1.DataSource = dt;
            cnn.Close();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)//вывод в текстовые поля
        {
            try
            {
                textBox9.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                textBox10.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();

                var date = DateTime.ParseExact(dataGridView1.CurrentRow.Cells[3].Value.ToString(), "dd.MM.yyyy h:mm:ss", null);//меняем формат даты
                maskedTextBox1.Text = date.ToString("yyyy-MM-dd");//вывод в поле

                checkBox1.Checked = Convert.ToBoolean(dataGridView1.CurrentRow.Cells[5].Value);
                checkBox2.Checked = Convert.ToBoolean(dataGridView1.CurrentRow.Cells[6].Value);
                textBox11.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                maskedTextBox2.Text = dataGridView1.CurrentRow.Cells[7].Value.ToString();
                textBox8.Text = dataGridView1.CurrentRow.Cells[8].Value.ToString();
            }
            catch(Exception exc)
            {

            }
        }

        private void button9_Click(object sender, EventArgs e)//удаляем по условию
        {
            try
            {

                //предупреждение об удалении записей
                if (MessageBox.Show("Are you sure you want to delete selected records?", "Attention", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    cnn.Open();//открываем подключение к бд        
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "DELETE FROM Chain_Cafes WHERE " + comboBox1.Text + " LIKE '%" + textBox1.Text + "%'";//запрос на удаление строк
                    //MessageBox.Show(cmd.CommandText);
                    cmd.Connection = cnn;
                    Int32 i = cmd.ExecuteNonQuery();//удаляем строки и получаем число сколько их удалили


                    MessageBox.Show("The following number of records has been deleted: " + i.ToString(),"Message",MessageBoxButtons.OK,MessageBoxIcon.Information  );

                    //теперь надо перерисовать
                    cmd.CommandText = "SELECT * FROM Chain_Cafes";//запрос на выборку строк
                    DataTable dt = new DataTable();
                    SqlDataAdapter ad = new SqlDataAdapter();
                    ad.SelectCommand = cmd;
                    ad.Fill(dt);
                    dataGridView1.DataSource = dt;



                }

            }
            catch (Exception exc)
            {
                MessageBox.Show("Unexpected error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            cnn.Close();//закрываем подключение к бд

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "Date")//ЕСЛИ НУЖНА ВЫБОРКА ПО ДАТЕ
            {
                maskedTextBox3.Mask = "0000-00-00";
                maskedTextBox4.Mask = "0000-00-00";
                maskedTextBox3.Text = "";//очистить текст
                //maskedTextBox4.Text = "";
            }
            else
            {
                maskedTextBox3.Text = "";
                maskedTextBox4.Text = "";
                maskedTextBox3.Mask = "";
                maskedTextBox4.Mask = "";
            }
        }

        private void button8_Click(object sender, EventArgs e)//изменение в соотвествии с условием
        {
            try//пробуем изменить запись
            {
                cnn.Open();
                Int32 i = dataGridView1.CurrentRow.Index;//получаем индекс выбранной строки
                String text;

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;

                if (comboBox3.Text == "Name" || comboBox3.Text == "Time" || comboBox3.Text == "Date")//для этих полей таблицы значения должны быть в ''
                {
                    text = "'" + textBox2.Text + "'";
                }
                else
                {
                    text = textBox2.Text;
                }

                cmd.CommandText = "UPDATE Chain_Cafes SET " + comboBox3.Text + " = " + text + " WHERE " + comboBox1.Text + " LIKE '%" + textBox1.Text + "%'";//запрос на изменение НЕСКОЛЬКИХ записей в бд в СООТВЕТСТВИИ с условием
                //MessageBox.Show(cmd.CommandText);
                MessageBox.Show("Rows affected: " + cmd.ExecuteNonQuery(), "Changes applied", MessageBoxButtons.OK, MessageBoxIcon.Information);//выполнить sql запрос и вывести сообщение

                //перерисовка таблицы
                cmd.CommandText = "SELECT * FROM Chain_Cafes";
                DataTable dt = new DataTable();
                SqlDataAdapter ad = new SqlDataAdapter();
                ad.SelectCommand = cmd;
                ad.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.Rows[i].Selected = true;//возвращаем фокус на строку
                
                //MessageBox.Show(cmd.CommandText);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Invalid input", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);//сообщение о некорректности ввода данных
            }

            cnn.Close();

        }
    }
}
