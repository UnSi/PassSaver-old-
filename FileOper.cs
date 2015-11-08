﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PassSaver
{
	static class FileOper {
		public static string fileName;
		public static bool CheckIfFileIsBeingUsed(string fileName)
		{
			try
			{
				var myFile = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
				myFile.Close();
			}
			catch
			{
				return true;
			}

			return false;
		}

		public static void CheckFolders()
		{
			if (!Directory.Exists("Error"))
				Directory.CreateDirectory("Error");
			if (!Directory.Exists("Backup"))
				Directory.CreateDirectory("Backup");
			if (!Directory.Exists("Wtf"))
				Directory.CreateDirectory("Wtf");
		}

		public static void loadArray(String filename)
		{
			if (Form1.first)
				Form1.lenEn();
			if (!File.Exists(filename)) //Возможно передать: /saver.dat/, /Backup/saver.dat/. 
                return;
			string line; // Это я загуглил. Так крутые проггеры делают
            List<String> list = new List<string>(); //List - откорректированный список загружаемых настроек.
            StreamReader sr = new StreamReader(File.OpenRead(filename)); //открыл файл настроек
            Form1.countArrSiteName = 0; //ВОТ ЭТОТ СЧЁТЧИК! Не зря обнулил.
            try // и если этот файл никто не ковырял...
			{
				while ((line = sr.ReadLine()) != "%EndFile%") //Читаю каждую строку до %конца файла%
				{
					if (line.Contains("<S| ")) countArrSiteName++; //Если сайт - увеличить счётчик  //  
                    if ((line != "") && (line != "%Endl%") && (line != null)) // форматирование строк
					{
						if ((!line.Contains("<S| ")) && (list.Count % 91 == 0)&&(!line.Contains("%l++"))) // Если на позиции сайта вовсе не сайт...
						{
							countArrSiteName++;
							MessageBox.Show("Error in " + Convert.ToInt32(list.Count / 91) + "site");
						}

						if (line.Contains("<S| ") && (list.Count % 91 != 0)) // Если сайт не на своём месте
						{
							line = line.Replace("<S| ", "");
							line = line.Replace(" />", "");
							MessageBox.Show("Error loading site " + line, Messages[5]); // выводим текущий сайт
                            while (list.Count % 91 != 0)
							{
								list.Add("Error. Load the backup!"); // заболняем "ошибочными значениями" все поля.
							}
						}

						line = line.Replace("<S| ", "");
						line = line.Replace("<L| ", "");
						line = line.Replace("<P| ", "");
						line = line.Replace("<M| ", "");
						line = line.Replace(" />", "");
						list.Add(line); //Наконец запилить это всё дело в список
					}
				}

				CheckFolders(); // Проверка наличия нужных программе папок. Если чё, создадим :)
				sr.Close(); // Вечно забываемая штука. ЗАКРЫТЬ ФАЙЛ!
                if (countArrSiteName < 6) // Если сайтов меньше 6-ти, выводим ошибку.
				{
					ErrorLoad(filename);
					MessageBox.Show(Form1.Messages[13], Form1.Messages[5]);
					return;
				}

				for (int i = 0, k = 0; i < countArrSiteName; i++) // Если файл ковырялся - счётчик не увиличится, цикл пропустится
				{
					if (list[k] != "")
						arr[i, 0, 0] = list[k++]; // Разделение списка на 3-хмерный массив
					else
						arr[i, 0, 0] = "Было сдвинуто значение!" + k++;
					for (int j = 0; j < arr.GetLength(1); j++) // цикл пробежится 30*countArrSiteName раз. Максимум - 6000. Это будет беда
					{
						arr[i, j, 1] = list[k++];
						arr[i, j, 2] = list[k++];
						arr[i, j, 3] = list[k++];
					}

					if (list[k].Contains("%l++")) // Загрузка языка
					{
						language = list[k].Replace("%l++", "");
						if (!first)
						{
							MessageBox.Show(Form1.Messages[6] + filename + Form1.Messages[7]);
							first = false;
						}

						formUpdate();
						break;
					}
				}
			}
			catch // ну, а если ковырял
			{
				sr.Close(); // ЗАКРЫТЬ ФАЙЛ!
                ErrorLoad(filename);
			}
		}

		public static void ErrorLoad(string filename)
		{
			FileOper.CheckFolders(); // Проверка папок
            MessageBox.Show(Form1.Messages[4], Form1.Messages[5]);
			//var MyFile = File.Create("Wtf/" + filename); // И так сойдёт
			//  MyFile.Close();
			string s = DateTime.Now.ToString();
			s = s.Replace(":", "-");
			s = filename + s;
			s = s.Replace(".dat", "");
			s = s + ".dat";
			File.Copy(filename, ("Wtf/" + s)); //Копирование файла в папку втф
            File.Delete(filename); // удаление файла
		}

		public static void saveArray(String fileName)
		{
			fileName = fileName.Replace(":", "-"); // В файле ":" неприемлимо
            CheckFolders(); //проверка и создание папок
            if (!File.Exists(fileName)) // Если файла нет - создаем.
			{
				var myFile = File.Create(fileName);
				myFile.Close();
			}

			FileStream file = new FileStream(fileName, FileMode.Create);//создаем файловый поток
            StreamWriter writer = new StreamWriter(file);//создаем «потоковый писатель» и связываем его с файловым потоком 
            for (int i = 0; i < cbSiteName.Items.Count; i++)
			{
				writer.WriteLine("<S| " + arr[i, 0, 0] + " />"); //записываем в файл

                for (int j = 0; j < arr.GetLength(1); j++)
				{
					writer.WriteLine("<L| " + arr[i, j, 1] + " />");
					writer.WriteLine("<P| " + arr[i, j, 2] + " />");
					writer.WriteLine("<M| " + arr[i, j, 3] + " />");
					writer.WriteLine();
				}

				writer.WriteLine("%Endl%");
			}

			writer.WriteLine("%l++" + language);
			writer.WriteLine("%EndFile%");
			writer.Close();
			MessageBox.Show(Form1.Messages[6] + fileName + Form1.Messages[8]);
		}

		public static void saveArray(String fileName, String param)
		{
			fileName = fileName.Replace(":", "-"); // В файле ":" неприемлимо
            CheckFolders(); //проверка и создание папок
            if (!File.Exists(fileName)) // Если файла нет - создаем.
			{
				var myFile = File.Create(fileName);
				myFile.Close();
			}

			FileStream file = new FileStream(fileName, FileMode.Create);//создаем файловый поток
            StreamWriter writer = new StreamWriter(file);//создаем «потоковый писатель» и связываем его с файловым потоком 
            for (int i = 0; i < cbSiteName.Items.Count; i++)
			{
				writer.WriteLine("<S| " + arr[i, 0, 0] + " />"); //записываем в файл

                for (int j = 0; j < arr.GetLength(1); j++)
				{
					writer.WriteLine("<L| " + arr[i, j, 1] + " />");
					writer.WriteLine("<P| " + arr[i, j, 2] + " />");
					writer.WriteLine("<M| " + arr[i, j, 3] + " />");
					writer.WriteLine();
				}

				writer.WriteLine("%Endl%");
			}

			writer.WriteLine("%l++" + language);
			writer.WriteLine("%EndFile%");
			writer.Close();
			if (param == "OnClosing")
				notifyIcon.ShowBalloonTip(1000, Form1.Messages[6]+fileName, Form1.Messages[8], ToolTipIcon.Info);
			else
				MessageBox.Show(Form1.Messages[6] + fileName + Form1.Messages[8]);
		}
	}
}