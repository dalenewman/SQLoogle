using NUnit.Framework;
using Sqloogle.Operations;

namespace Tests
{
    [TestFixture]
    public class TestSqlParameterReplacer
    {
        [Test]
        public void TestReplaceSimpleParameters() {

            const string sql = @"
                select 0 from t where c = @z;
                select 1 from t where c = 1
                select 2 from t where c = 15.1
                select 3 from t where c = 1.
                select 4 from t where c = .15
                select 5 from t where c=2
                select 6 from t where c > 1
                select 7 from t where c < 1
                select 8 from t where c <> 1
                select 9 from t where c != 1
                select 10 from t where c != 1;
                select 11 from t where c1 = 1 and c2 = 2;
                select 12 from t where c1 = 'dale' and c2 = 3.0
                select 13 from t where c = 'Newman'
                select 14 from t where c1 = '' and c2 = 3
                select 15 from t where c1 = 'stuff' and c2 = '3'
                select 17 from t where c like 'something%'
                select 18 from t where c not like 'something%'
                select 19 from t where c = 'dale''s'
                select 20 from t where c1 = 'dale''s' and c2 = 'x'
                select 21 from t where c = -1
                select 22 from t where c LIKE '%something%'
                select 23 from t where ""t"".""c""='0000089158'
                select 24 from t1 inner join t2 on (t1.field = t2.field) where t2.field = 'x';
                select 25 from t where c = @Parameter
                select 26 from t where c = N'Something'
                select 27 from t1 inner join t2 on (""t1"".""field""=""t2"".""field"") where double_quotes = 'no'
                select 28 from t where [c]='brackets!';
                select 29 from t where x = 0x261BE3E63BBFD8439B5CE2971D70A5DE;
            ";

            const string expected = @"
                select 0 from t where c = @z;
                select 1 from t where c = @Parameter
                select 2 from t where c = @Parameter
                select 3 from t where c = @Parameter
                select 4 from t where c = @Parameter
                select 5 from t where c= @Parameter
                select 6 from t where c > @Parameter
                select 7 from t where c < @Parameter
                select 8 from t where c <> @Parameter
                select 9 from t where c != @Parameter
                select 10 from t where c != @Parameter;
                select 11 from t where c1 = @Parameter and c2 = @Parameter;
                select 12 from t where c1 = @Parameter and c2 = @Parameter
                select 13 from t where c = @Parameter
                select 14 from t where c1 = @Parameter and c2 = @Parameter
                select 15 from t where c1 = @Parameter and c2 = @Parameter
                select 17 from t where c like @Parameter
                select 18 from t where c not like @Parameter
                select 19 from t where c = @Parameter
                select 20 from t where c1 = @Parameter and c2 = @Parameter
                select 21 from t where c = @Parameter
                select 22 from t where c LIKE @Parameter
                select 23 from t where ""t"".""c""= @Parameter
                select 24 from t1 inner join t2 on (t1.field = t2.field) where t2.field = @Parameter;
                select 25 from t where c = @Parameter
                select 26 from t where c = @Parameter
                select 27 from t1 inner join t2 on (""t1"".""field""=""t2"".""field"") where double_quotes = @Parameter
                select 28 from t where [c]= @Parameter;
                select 29 from t where x = @Parameter;
            ";

            var actual = CachedSqlPreTransform.ReplaceParameters(sql);
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void TestQueriesWithDifferentParameterValuesEndUpTheSame1() {
            const string sql1 = "UPDATE ORDERS SET Exported=1 WHERE (OrderNumber=11844259) AND Store_ID = 1";
            const string sql2 = "UPDATE ORDERS SET Exported=1 WHERE (OrderNumber=234985793) AND Store_ID = 1";

            Assert.AreEqual(CachedSqlPreTransform.ReplaceParameters(sql1), CachedSqlPreTransform.ReplaceParameters(sql2));

        }

        [Test]
        public void TestQueriesWithDifferentParameterValuesEndUpTheSame2()
        {
            const string sql1 = "SELECT count(*) FROM Country WHERE (Country.CountryCode LIKE '%b%' OR Country.CountryName LIKE '%b%');";
            const string sql2 = "SELECT count(*) FROM Country WHERE (Country.CountryCode LIKE '%bel%' OR Country.CountryName LIKE '%bel%');";

            Assert.AreEqual(CachedSqlPreTransform.ReplaceParameters(sql1), CachedSqlPreTransform.ReplaceParameters(sql2));
        }

        [Test]
        public void TestQueriesWithDifferentParameterValuesEndUpTheSame3()
        {
            const string sql1 = "SELECT \"Customer\".\"Name\", \"Customer\".\"Addr1\", \"Customer\".\"Addr2\", \"pjaddr\".\"addr_key_cd\", \"pjaddr\".\"addr_type_cd\", \"pjaddr\".\"addr_key\", \"pjinvhdr\".\"project_billwith\", \"Customer\".\"Attn\", \"Customer\".\"City\", \"Customer\".\"State\", \"Customer\".\"Zip\", \"Customer\".\"Country\", \"CustCountry\".\"Descr\", \"CustBillCountry\".\"Descr\", \"ProjBillCountry\".\"Descr\", \"pjinvhdr\".\"draft_num\", \"pjinvhdr\".\"customer\", \"pjproj\".\"customer\", \"pjaddr\".\"addr1\", \"pjaddr\".\"addr2\", \"pjaddr\".\"city\", \"pjaddr\".\"state\", \"pjaddr\".\"zip\", \"pjaddr\".\"country\", \"pjaddr\".\"comp_name\", \"pjaddr\".\"individual\", \"Customer\".\"BillAddr1\", \"Customer\".\"BillAddr2\", \"Customer\".\"BillCity\", \"Customer\".\"BillState\", \"Customer\".\"BillZip\", \"Customer\".\"BillCountry\", \"Customer\".\"BillName\", \"Customer\".\"BillAttn\" FROM   (((((\"ScopeApp1\".\"dbo\".\"PJINVHDR\" \"pjinvhdr\" INNER JOIN \"ScopeApp1\".\"dbo\".\"Customer\" \"Customer\" ON \"pjinvhdr\".\"customer\"=\"Customer\".\"CustId\") LEFT OUTER JOIN \"ScopeApp1\".\"dbo\".\"PJADDR\" \"pjaddr\" ON \"pjinvhdr\".\"project_billwith\"=\"pjaddr\".\"addr_key\") LEFT OUTER JOIN \"ScopeApp1\".\"dbo\".\"PJPROJ\" \"pjproj\" ON \"pjinvhdr\".\"project_billwith\"=\"pjproj\".\"project\") LEFT OUTER JOIN \"ScopeApp1\".\"dbo\".\"Country\" \"CustCountry\" ON \"Customer\".\"Country\"=\"CustCountry\".\"CountryID\") LEFT OUTER JOIN \"ScopeApp1\".\"dbo\".\"Country\" \"CustBillCountry\" ON \"Customer\".\"BillCountry\"=\"CustBillCountry\".\"CountryID\") LEFT OUTER JOIN \"ScopeApp1\".\"dbo\".\"Country\" \"ProjBillCountry\" ON \"pjaddr\".\"country\"=\"ProjBillCountry\".\"CountryID\" WHERE  \"pjinvhdr\".\"draft_num\"='0000089158'";
            const string sql2 = "SELECT \"Customer\".\"Name\", \"Customer\".\"Addr1\", \"Customer\".\"Addr2\", \"pjaddr\".\"addr_key_cd\", \"pjaddr\".\"addr_type_cd\", \"pjaddr\".\"addr_key\", \"pjinvhdr\".\"project_billwith\", \"Customer\".\"Attn\", \"Customer\".\"City\", \"Customer\".\"State\", \"Customer\".\"Zip\", \"Customer\".\"Country\", \"CustCountry\".\"Descr\", \"CustBillCountry\".\"Descr\", \"ProjBillCountry\".\"Descr\", \"pjinvhdr\".\"draft_num\", \"pjinvhdr\".\"customer\", \"pjproj\".\"customer\", \"pjaddr\".\"addr1\", \"pjaddr\".\"addr2\", \"pjaddr\".\"city\", \"pjaddr\".\"state\", \"pjaddr\".\"zip\", \"pjaddr\".\"country\", \"pjaddr\".\"comp_name\", \"pjaddr\".\"individual\", \"Customer\".\"BillAddr1\", \"Customer\".\"BillAddr2\", \"Customer\".\"BillCity\", \"Customer\".\"BillState\", \"Customer\".\"BillZip\", \"Customer\".\"BillCountry\", \"Customer\".\"BillName\", \"Customer\".\"BillAttn\" FROM   (((((\"ScopeApp1\".\"dbo\".\"PJINVHDR\" \"pjinvhdr\" INNER JOIN \"ScopeApp1\".\"dbo\".\"Customer\" \"Customer\" ON \"pjinvhdr\".\"customer\"=\"Customer\".\"CustId\") LEFT OUTER JOIN \"ScopeApp1\".\"dbo\".\"PJADDR\" \"pjaddr\" ON \"pjinvhdr\".\"project_billwith\"=\"pjaddr\".\"addr_key\") LEFT OUTER JOIN \"ScopeApp1\".\"dbo\".\"PJPROJ\" \"pjproj\" ON \"pjinvhdr\".\"project_billwith\"=\"pjproj\".\"project\") LEFT OUTER JOIN \"ScopeApp1\".\"dbo\".\"Country\" \"CustCountry\" ON \"Customer\".\"Country\"=\"CustCountry\".\"CountryID\") LEFT OUTER JOIN \"ScopeApp1\".\"dbo\".\"Country\" \"CustBillCountry\" ON \"Customer\".\"BillCountry\"=\"CustBillCountry\".\"CountryID\") LEFT OUTER JOIN \"ScopeApp1\".\"dbo\".\"Country\" \"ProjBillCountry\" ON \"pjaddr\".\"country\"=\"ProjBillCountry\".\"CountryID\" WHERE  \"pjinvhdr\".\"draft_num\"='0000089159'";

            Assert.AreEqual(CachedSqlPreTransform.ReplaceParameters(sql1), CachedSqlPreTransform.ReplaceParameters(sql2));
        }


 

    }
}
