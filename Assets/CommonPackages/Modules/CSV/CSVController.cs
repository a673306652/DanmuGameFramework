namespace Modules.CSV
{
	using System.Collections.Generic;
	using System.IO;

	public class CSVController
	{

		static CSVController csv;
		public List<string []> arrayData;

		private CSVController() //单例，构造方法为私有
		{
			arrayData = new List<string []>();
		}

		public static CSVController GetInstance() //单例方法获取对象
		{
			if (csv == null)
			{
				csv = new CSVController();
			}
			return csv;
		}

		public void LoadFile(string path, string fileName)
		{
			arrayData.Clear();
			StreamReader sr = null;
			try
			{
				string file_url = path + "//" + fileName;
				sr = File.OpenText(file_url);
			}
			catch
			{
				return;
			}

			string line;
			while ((line = sr.ReadLine()) != null)
			{
				arrayData.Add(line.Split(','));
			}
			sr.Close();
			sr.Dispose();
		}

		public string GetString(int row, int col)
		{
			return arrayData [row] [col];
		}
		public int GetInt(int row, int col)
		{
			return int.Parse(arrayData [row] [col]);
		}
	}
}