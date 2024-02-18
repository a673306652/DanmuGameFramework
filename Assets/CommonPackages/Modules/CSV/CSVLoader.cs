namespace Modules.CSV
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;

    /*===============================================================*/
    /**
     * CSVを読み込むクラス
     * 2014年12月23日 Buravo
     */
    public class CSVLoader
    {
        /*===============================================================*/
        /**
         * @brief コンストラクタ
         */
        public CSVLoader()
        {
            this.Initialize();
        }

        /*===============================================================*/
        /*===============================================================*/
        /**
         * @brief 初期化
         */
        public void Initialize()
        {
        }

        /*===============================================================*/
        /*===============================================================*/
        /**
         * @brief 実行処理
         */
        public void Execution()
        {
        }


        private static string[] SplitIgnoringQuotes(string input, char separator)
        {
            string pattern = $@"{separator}(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
            return Regex.Split(input, pattern);
        }
        /*===============================================================*/
        /*===============================================================*/
        /**
         * @brief CSVを読み込んで、レコードを所持するデータテーブルを渡す関数
         * @param string 読み込むCSVのファイルパス
         * @return CSVTable CSVのデータテーブルクラス
         */
        public CSVTable LoadCSV(string t_csv_path)
        {
            // テキストアセットとしてCSVをロード.
            TextAsset csvTextAsset = Resources.Load(t_csv_path) as TextAsset;
            return LoadFromTextAsset(csvTextAsset.text);
        }

        public CSVTable LoadCSVEncrypted(string t_csv_path)
        {
            // テキストアセットとしてCSVをロード.
            TextAsset csvTextAsset = Resources.Load(t_csv_path) as TextAsset;
            return LoadFromTextAsset(Database.AESEncryptor.Decrypt(csvTextAsset.text, CSVKeys.EncryptionKS));
        }

        public CSVTable LoadFromRawText(string rawCSVText)
        {
            return LoadFromTextAsset(rawCSVText);
        }

        private CSVTable LoadFromTextAsset(string rawCSVText)
        {
            // データテーブルクラスの生成.
            CSVTable csvTable = new CSVTable();
            // OS環境ごとに適切な改行コードをCR(=キャリッジリターン)に置換.
            string csvText =
                rawCSVText.Replace(Environment.NewLine, "\r");

            // テキストデータの前後からCRを取り除く.
            csvText = csvText.Trim('\r');
            csvText = csvText.Replace("\r\r", "\r");

            // CRを区切り文字として分割して配列に変換.
            // string[] csv = csvText.Split('\r');
            string[] csv = SplitIgnoringQuotes(csvText, '\n');
            if (csv.Length <= 1)
            {
                Debug.Log($"警告：csv行数为 <{csv.Length}>: 确定分隔符无误？");
                csv = SplitIgnoringQuotes(csvText, '\r');
            }

            // 複数の行を元にリストの生成.
            List<string> rows = new List<string>(csv);

            // 項目名の取得.
            // string[] headers = rows[0].Split(',');
            // string[] comments = rows[1].Split(',');
            string[] headers = SplitIgnoringQuotes(rows[0], ',');
            string[] comments = SplitIgnoringQuotes(rows[1], ',');



            // 項目の格納.
            for (var h = 0; h < headers.Length; h++)
            {
                var header = headers[h];
                if (header.StartsWith("||")) continue;

                csvTable.AddHeaders(header);
                csvTable.AddHeaderComment(comments[h]);
            }

            // 項目名の削除.
            rows.RemoveAt(0);
            rows.RemoveAt(0);

            // 1件分のデータであるレコードを生成して追加.
            foreach (string row in rows)
            {
                if (row.StartsWith("||")) continue;

                // 各項目の値へと分割.
                // string[] fields = row.Split(',');
                string[] fields = SplitIgnoringQuotes(row, ',');

                // レコードを追加.
                csvTable.AddRecord(CreateRecord(csvTable.Headers, fields));
            }
            return csvTable;
        }

        public CSVTable LoadCSV(TextAsset textAsset)
        {
            CSVTable csvTable = new CSVTable();
            string csvText = textAsset.text.Replace(Environment.NewLine, "\r");

            // テキストデータの前後からCRを取り除く.
            csvText = csvText.Trim('\r');
            csvText = csvText.Replace("\r\r", "\r");

            // CRを区切り文字として分割して配列に変換.
            string[] csv = csvText.Split('\r');

            // 複数の行を元にリストの生成.
            List<string> rows = new List<string>(csv);

            // 項目名の取得.
            string[] headers = rows[0].Split(',');
            string[] comments = rows[1].Split(',');

            // 項目の格納.
            for (var h = 0; h < headers.Length; h++)
            {
                var header = headers[h];
                if (header.Contains("||")) continue;

                csvTable.AddHeaders(header);
                csvTable.AddHeaderComment(comments[h]);
            }

            // 項目名の削除.
            rows.RemoveAt(0);
            rows.RemoveAt(0);

            // 1件分のデータであるレコードを生成して追加.
            foreach (string row in rows)
            {
                if (row.Contains("||")) continue;

                // 各項目の値へと分割.
                string[] fields = row.Split(',');

                // レコードを追加.
                csvTable.AddRecord(CreateRecord(csvTable.Headers, fields));
            }
            return csvTable;
        }

        /*===============================================================*/
        /*===============================================================*/
        /**
         * @brief 項目名をキーに入力項目を格納するレコードを生成する関数
         * @param string[] 項目名
         * @param string[] 入力項目
         * @return CSVRecord 項目名をキーに入力項目を格納するレコード
         */
        private CSVRecord
        CreateRecord(List<string> t_headers, string[] t_fields)
        {
            // レコードを生成.
            CSVRecord record = new CSVRecord();

            // 項目名をキーに入力項目をレコードへ格納.
            for (int i = 0; i < t_headers.Count; ++i)
            {
                record.AddField(t_headers[i], t_fields[i]);
            }
            return record;
        }
        /*===============================================================*/
    }
    /*===============================================================*/
}
