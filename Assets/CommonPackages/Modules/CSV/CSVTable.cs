namespace Modules.CSV
{
    using System.Collections.Generic;

    /*===============================================================*/
    /**
	 * CSVのレコードを管理するデータテーブルクラス
	 * 2014年12月23日 Buravo
	 */
    public class CSVTable
    {
        /*===============================================================*/
        /**
		 * @brief 項目名を管理するリスト
		 */
        private List<string> m_headers = new List<string>();

        private List<string> m_headerCMTs = new List<string>();

        /**
		 * @brief レコードを管理するリスト
		 */
        private List<CSVRecord> m_records = new List<CSVRecord>();

        /*===============================================================*/
        /*===============================================================*/
        /**
		 * @brief 項目名を管理するリスト
		 */
        public List<string> Headers
        {
            get
            {
                return m_headers;
            }
        }

        public List<string> HeaderComments
        {
            get
            {
                return m_headerCMTs;
            }
        }

        /**
		 * @brief レコードを管理するリスト
		 */
        public List<CSVRecord> Records
        {
            get
            {
                return m_records;
            }
        }

        /*===============================================================*/
        /*===============================================================*/
        /**
		 * @brief コンストラクタ
		 */
        public CSVTable()
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

        /*===============================================================*/
        /*===============================================================*/
        /**
		 * @brief 項目名の追加.
		 * @param string 項目名
		 */
        public void AddHeaders(string t_header)
        {
            m_headers.Add (t_header);
        }

        public void AddHeaderComment(string t_hCMT)
        {
            m_headerCMTs.Add (t_hCMT);
        }

        /*===============================================================*/
        /*===============================================================*/
        /**
		 * @brief レコードの追加.
		 * @param CSVRecord レコード
		 */
        public void AddRecord(CSVRecord t_record)
        {
            m_records.Add (t_record);
        }

        /*===============================================================*/
        /*===============================================================*/
        /**
		 * @brief レコードの取得.
		 * @param int レコードの番号
		 * @return CSVRecord レコード
		 */
        public CSVRecord GetRecord(int t_record_number)
        {
            return m_records[t_record_number];
        }
        /*===============================================================*/
    }
    /*===============================================================*/
}
