using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SobekCM.Core.Database
{
    /// <summary> Static library used for synchronous reads from a database, agnostic of the type of 
    /// database engine ( i.e., MS SQL, PostgreSQL, etc.. ) </summary>
    public static class EalDbAccess
    {
        /// <summary> Test to see if a connection string is valid and can be used to create a connection </summary>
        /// <param name="DbType"> Type of database ( i.e., MSSQL, PostgreSQL ) </param>
        /// <param name="DbConnectionString"> Database connection string </param>
        /// <returns> TRUE if valid and accepting connections, otherwise FALSE </returns>
        public static bool Test(EalDbTypeEnum DbType, string DbConnectionString)
        {
            if (DbType == EalDbTypeEnum.MSSQL)
            {
                // Create the SQL connection
                using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
                {
                    try
                    {
                        sqlConnect.Open();
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                    
                    // Close the connection (not technical necessary since we put the connection in the
                    // scope of the using brackets.. it would dispose itself anyway)
                    try
                    {
                        sqlConnect.Close();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }

                // SUCCESS!
                return true;
            }

            if (DbType == EalDbTypeEnum.PostgreSQL)
            {
                throw new ApplicationException("Support for PostgreSQL with SobekCM is targeted for early 2016");
            }

            throw new ApplicationException("Unknown database type not supported");
        }

        /// <summary> Execute a non-query SQL statement or stored procedure </summary>
        /// <param name="DbType"> Type of database ( i.e., MSSQL, PostgreSQL ) </param>
        /// <param name="DbConnectionString"> Database connection string </param>
        /// <param name="DbCommandType"> Database command type </param>
        /// <param name="DbCommandText"> Text of the database command, or name of the stored procedure to run </param>
        public static void ExecuteNonQuery(EalDbTypeEnum DbType, string DbConnectionString, CommandType DbCommandType, string DbCommandText)
        {
            ExecuteNonQuery(DbType, DbConnectionString, DbCommandType, DbCommandText, new EalDbParameter[0]);
        }

        /// <summary> Execute a non-query SQL statement or stored procedure </summary>
        /// <param name="DbType"> Type of database ( i.e., MSSQL, PostgreSQL ) </param>
        /// <param name="DbConnectionString"> Database connection string </param>
        /// <param name="DbCommandType"> Database command type </param>
        /// <param name="DbCommandText"> Text of the database command, or name of the stored procedure to run </param>
        /// <param name="DbParameters"> Parameters for the SQL statement </param>
        public static void ExecuteNonQuery(EalDbTypeEnum DbType, string DbConnectionString, CommandType DbCommandType, string DbCommandText, List<EalDbParameter> DbParameters)
        {
            if (DbType == EalDbTypeEnum.MSSQL)
            {
                // Create the SQL connection
                using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
                {
                    try
                    {
                        sqlConnect.Open();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Unable to open connection to the database." + Environment.NewLine + ex.Message, ex);
                    }

                    // Create the SQL command
                    SqlCommand sqlCommand = new SqlCommand(DbCommandText, sqlConnect)
                    {
                        CommandType = DbCommandType
                    };

                    // Copy all the parameters to this adapter
                    sql_add_params_to_command(sqlCommand, DbParameters);

                    // Run the command itself
                    try
                    {
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Error executing non-query command." + Environment.NewLine + ex.Message, ex);
                    }

                    // Copy any output values back to the parameters
                    sql_copy_returned_values_back_to_params(sqlCommand.Parameters, DbParameters);

                    // Close the connection (not technical necessary since we put the connection in the
                    // scope of the using brackets.. it would dispose itself anyway)
                    try
                    {
                        sqlConnect.Close();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Unable to close connection to the database." + Environment.NewLine + ex.Message, ex);
                    }
                }

                // Return
                return;
            }

            if (DbType == EalDbTypeEnum.PostgreSQL)
            {
                throw new ApplicationException("Support for PostgreSQL with SobekCM is targeted for early 2016");
            }

            throw new ApplicationException("Unknown database type not supported");
        }

        /// <summary> Execute a non-query SQL statement or stored procedure </summary>
        /// <param name="DbType"> Type of database ( i.e., MSSQL, PostgreSQL ) </param>
        /// <param name="DbConnectionString"> Database connection string </param>
        /// <param name="DbCommandType"> Database command type </param>
        /// <param name="DbCommandText"> Text of the database command, or name of the stored procedure to run </param>
        /// <param name="DbParameters"> Parameters for the SQL statement </param>
        public static void ExecuteNonQuery(EalDbTypeEnum DbType, string DbConnectionString, CommandType DbCommandType, string DbCommandText, EalDbParameter[] DbParameters)
        {
            if (DbType == EalDbTypeEnum.MSSQL)
            {
                // Create the SQL connection
                using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
                {
                    try
                    {
                        sqlConnect.Open();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Unable to open connection to the database." + Environment.NewLine + ex.Message, ex);
                    }

                    // Create the SQL command
                    SqlCommand sqlCommand = new SqlCommand(DbCommandText, sqlConnect)
                    {
                        CommandType = DbCommandType
                    };

                    // Copy all the parameters to this adapter
                    sql_add_params_to_command(sqlCommand, DbParameters);

                    // Run the command itself
                    try
                    {
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Error executing non-query command." + Environment.NewLine + ex.Message, ex);
                    }

                    // Copy any output values back to the parameters
                    sql_copy_returned_values_back_to_params(sqlCommand.Parameters, DbParameters);

                    // Close the connection (not technical necessary since we put the connection in the
                    // scope of the using brackets.. it would dispose itself anyway)
                    try
                    {
                        sqlConnect.Close();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Unable to close connection to the database." + Environment.NewLine + ex.Message, ex);
                    }
                }

                // Return
                return;
            }

            if (DbType == EalDbTypeEnum.PostgreSQL)
            {
                throw new ApplicationException("Support for PostgreSQL with SobekCM is targeted for early 2016");
            }

            throw new ApplicationException("Unknown database type not supported");
        }
        
        /// <summary> Execute a SQL statement or stored procedure and return a DataSet </summary>
        /// <param name="DbType"> Type of database ( i.e., MSSQL, PostgreSQL ) </param>
        /// <param name="DbConnectionString"> Database connection string </param>
        /// <param name="DbCommandType"> Database command type </param>
        /// <param name="DbCommandText"> Text of the database command, or name of the stored procedure to run </param>
        public static DataSet ExecuteDataset(EalDbTypeEnum DbType, string DbConnectionString, CommandType DbCommandType, string DbCommandText )
        {
            return ExecuteDataset(DbType, DbConnectionString, DbCommandType, DbCommandText, new EalDbParameter[0]);
        }

        /// <summary> Execute a SQL statement or stored procedure and return a DataSet </summary>
        /// <param name="DbType"> Type of database ( i.e., MSSQL, PostgreSQL ) </param>
        /// <param name="DbConnectionString"> Database connection string </param>
        /// <param name="DbCommandType"> Database command type </param>
        /// <param name="DbCommandText"> Text of the database command, or name of the stored procedure to run </param>
        /// <param name="DbParameters"> Parameters for the SQL statement </param>
        public static DataSet ExecuteDataset( EalDbTypeEnum DbType, string DbConnectionString, CommandType DbCommandType, string DbCommandText, EalDbParameter[] DbParameters)
        {
            if (DbType == EalDbTypeEnum.MSSQL)
            {
                DataSet returnedSet = new DataSet();

                // Create the SQL connection
                using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
                {
                    try
                    {
                        sqlConnect.Open();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Unable to open connection to the database." + Environment.NewLine + ex.Message, ex);
                    }

                    // Create the data adapter
                    SqlDataAdapter sqlAdapter = new SqlDataAdapter(DbCommandText, sqlConnect)
                    {
                        SelectCommand = { CommandType = DbCommandType }
                    };

                    // Copy all the parameters to this adapter
                    sql_add_params_to_command(sqlAdapter.SelectCommand, DbParameters);
                    
                    // Fill the dataset to return 
                    sqlAdapter.Fill(returnedSet);

                    // Copy any output values back to the parameters
                    sql_copy_returned_values_back_to_params(sqlAdapter.SelectCommand.Parameters, DbParameters);
                }

                // Return the dataset
                return returnedSet;
            }

            if (DbType == EalDbTypeEnum.PostgreSQL)
            {
                throw new ApplicationException("Support for PostgreSQL with SobekCM is targeted for early 2016");
            }

            throw new ApplicationException("Unknown database type not supported");
        }

        /// <summary> Execute a SQL statement or stored procedure and return a DataSet </summary>
        /// <param name="DbType"> Type of database ( i.e., MSSQL, PostgreSQL ) </param>
        /// <param name="DbConnectionString"> Database connection string </param>
        /// <param name="DbCommandType"> Database command type </param>
        /// <param name="DbCommandText"> Text of the database command, or name of the stored procedure to run </param>
        /// <param name="DbParameters"> Parameters for the SQL statement </param>
        public static DataSet ExecuteDataset(EalDbTypeEnum DbType, string DbConnectionString, CommandType DbCommandType, string DbCommandText, List<EalDbParameter> DbParameters)
        {
            if (DbType == EalDbTypeEnum.MSSQL)
            {
                DataSet returnedSet = new DataSet();

                // Create the SQL connection
                using (SqlConnection sqlConnect = new SqlConnection(DbConnectionString))
                {
                    try
                    {
                        sqlConnect.Open();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Unable to open connection to the database." + Environment.NewLine + ex.Message, ex);
                    }

                    // Create the data adapter
                    SqlDataAdapter sqlAdapter = new SqlDataAdapter(DbCommandText, sqlConnect)
                    {
                        SelectCommand = { CommandType = DbCommandType }
                    };

                    // Copy all the parameters to this adapter
                    sql_add_params_to_command(sqlAdapter.SelectCommand, DbParameters);

                    // Fill the dataset to return 
                    sqlAdapter.Fill(returnedSet);

                    // Copy any output values back to the parameters
                    sql_copy_returned_values_back_to_params(sqlAdapter.SelectCommand.Parameters, DbParameters);
                }

                // Return the dataset
                return returnedSet;
            }

            if (DbType == EalDbTypeEnum.PostgreSQL)
            {
                throw new ApplicationException("Support for PostgreSQL with SobekCM is targeted for early 2016");
            }

            throw new ApplicationException("Unknown database type not supported");
        }

        /// <summary> Execute a SQL statement or stored procedure and return a data reader </summary>
        /// <param name="DbType"> Type of database ( i.e., MSSQL, PostgreSQL ) </param>
        /// <param name="DbConnectionString"> Database connection string </param>
        /// <param name="DbCommandType"> Database command type </param>
        /// <param name="DbCommandText"> Text of the database command, or name of the stored procedure to run </param>
        public static EalDbReaderWrapper ExecuteDataReader(EalDbTypeEnum DbType, string DbConnectionString, CommandType DbCommandType, string DbCommandText)
        {
            return ExecuteDataReader(DbType, DbConnectionString, DbCommandType, DbCommandText, new EalDbParameter[0]);
        }

        /// <summary> Execute a SQL statement or stored procedure and return a data reader </summary>
        /// <param name="DbType"> Type of database ( i.e., MSSQL, PostgreSQL ) </param>
        /// <param name="DbConnectionString"> Database connection string </param>
        /// <param name="DbCommandType"> Database command type </param>
        /// <param name="DbCommandText"> Text of the database command, or name of the stored procedure to run </param>
        /// <param name="DbParameters"> Parameters for the SQL statement </param>
        public static EalDbReaderWrapper ExecuteDataReader(EalDbTypeEnum DbType, string DbConnectionString, CommandType DbCommandType, string DbCommandText, EalDbParameter[] DbParameters)
        {
            if (DbType == EalDbTypeEnum.MSSQL)
            {
                // Create the SQL connection
                SqlConnection sqlConnect = new SqlConnection(DbConnectionString);

                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to open connection to the database." + Environment.NewLine + ex.Message, ex);
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand(DbCommandText, sqlConnect)
                {
                    CommandType = DbCommandType
                };

                // Copy all the parameters to this adapter
                sql_add_params_to_command(sqlCommand, DbParameters);

                // Fill the dataset to return 
                SqlDataReader reader;

                // Try to open the reader.. if there was an error, close the database connection
                // before passing out the exception
                try
                {
                    reader = sqlCommand.ExecuteReader();
                }
                catch (Exception)
                {
                    sqlConnect.Close();
                    throw;
                }

                // Create the reader wrapper
                EalDbReaderWrapper returnValue = new EalDbReaderWrapper(sqlConnect, reader);

                // Copy any output values back to the parameters
                sql_copy_returned_values_back_to_params(returnValue, sqlCommand.Parameters, DbParameters);

                // Return the dataset
                return returnValue;
            }

            if (DbType == EalDbTypeEnum.PostgreSQL)
            {
                throw new ApplicationException("Support for PostgreSQL with SobekCM is targeted for early 2016");
            }

            throw new ApplicationException("Unknown database type not supported");
        }

        /// <summary> Execute a SQL statement or stored procedure and return a data reader </summary>
        /// <param name="DbType"> Type of database ( i.e., MSSQL, PostgreSQL ) </param>
        /// <param name="DbConnectionString"> Database connection string </param>
        /// <param name="DbCommandType"> Database command type </param>
        /// <param name="DbCommandText"> Text of the database command, or name of the stored procedure to run </param>
        /// <param name="DbParameters"> Parameters for the SQL statement </param>
        public static EalDbReaderWrapper ExecuteDataReader(EalDbTypeEnum DbType, string DbConnectionString, CommandType DbCommandType, string DbCommandText, List<EalDbParameter> DbParameters)
        {
            if (DbType == EalDbTypeEnum.MSSQL)
            {
                // Create the SQL connection
                SqlConnection sqlConnect = new SqlConnection(DbConnectionString);


                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to open connection to the database." + Environment.NewLine + ex.Message, ex);
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand(DbCommandText, sqlConnect)
                {
                    CommandType = DbCommandType
                };

                // Copy all the parameters to this adapter
                sql_add_params_to_command(sqlCommand, DbParameters);

                // Fill the dataset to return 
                SqlDataReader reader;

                // Try to open the reader.. if there was an error, close the database connection
                // before passing out the exception
                try
                {
                    reader = sqlCommand.ExecuteReader();
                }
                catch (Exception)
                {
                    sqlConnect.Close();
                    throw;
                }

                // Create the reader wrapper
                EalDbReaderWrapper returnValue = new EalDbReaderWrapper(sqlConnect, reader);

                // Copy any output values back to the parameters
                sql_copy_returned_values_back_to_params(returnValue, sqlCommand.Parameters, DbParameters);

                // Return the dataset
                return returnValue;
            }

            if (DbType == EalDbTypeEnum.PostgreSQL)
            {
                throw new ApplicationException("Support for PostgreSQL with SobekCM is targeted for early 2016");
            }

            throw new ApplicationException("Unknown database type not supported");
        }


        /// <summary> Execute an asynchronous non-query SQL statement or stored procedure </summary>
        /// <param name="DbType"> Type of database ( i.e., MSSQL, PostgreSQL ) </param>
        /// <param name="DbConnectionString"> Database connection string </param>
        /// <param name="DbCommandType"> Database command type </param>
        /// <param name="DbCommandText"> Text of the database command, or name of the stored procedure to run </param>
        public static void BeginExecuteNonQuery(EalDbTypeEnum DbType, string DbConnectionString, CommandType DbCommandType, string DbCommandText)
        {
            BeginExecuteNonQuery(DbType, DbConnectionString, DbCommandType, DbCommandText, new EalDbParameter[0]);
        }

        /// <summary> Execute an asynchronous non-query SQL statement or stored procedure </summary>
        /// <param name="DbType"> Type of database ( i.e., MSSQL, PostgreSQL ) </param>
        /// <param name="DbConnectionString"> Database connection string </param>
        /// <param name="DbCommandType"> Database command type </param>
        /// <param name="DbCommandText"> Text of the database command, or name of the stored procedure to run </param>
        /// <param name="DbParameters"> Parameters for the SQL statement </param>
        public static void BeginExecuteNonQuery(EalDbTypeEnum DbType, string DbConnectionString, CommandType DbCommandType, string DbCommandText, List<EalDbParameter> DbParameters)
        {
            if (DbType == EalDbTypeEnum.MSSQL)
            {
                // Create the SQL connection
                SqlConnection sqlConnect = new SqlConnection(DbConnectionString);
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to open connection to the database." + Environment.NewLine + ex.Message, ex);
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand(DbCommandText, sqlConnect)
                {
                    CommandType = DbCommandType
                };

                // Copy all the parameters to this adapter
                sql_add_params_to_command(sqlCommand, DbParameters);

                // Run the command itself
                try
                {
                    sqlCommand.BeginExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error executing non-query command." + Environment.NewLine + ex.Message, ex);
                }

                // Return
                return;
            }

            if (DbType == EalDbTypeEnum.PostgreSQL)
            {
                throw new ApplicationException("Support for PostgreSQL with SobekCM is targeted for early 2016");
            }

            throw new ApplicationException("Unknown database type not supported");
        }

        /// <summary> Execute an asynchronous non-query SQL statement or stored procedure </summary>
        /// <param name="DbType"> Type of database ( i.e., MSSQL, PostgreSQL ) </param>
        /// <param name="DbConnectionString"> Database connection string </param>
        /// <param name="DbCommandType"> Database command type </param>
        /// <param name="DbCommandText"> Text of the database command, or name of the stored procedure to run </param>
        /// <param name="DbParameters"> Parameters for the SQL statement </param>
        public static void BeginExecuteNonQuery(EalDbTypeEnum DbType, string DbConnectionString, CommandType DbCommandType, string DbCommandText, EalDbParameter[] DbParameters)
        {
            if (DbType == EalDbTypeEnum.MSSQL)
            {
                // Create the SQL connection
                SqlConnection sqlConnect = new SqlConnection(DbConnectionString);

                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to open connection to the database." + Environment.NewLine + ex.Message, ex);
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand(DbCommandText, sqlConnect)
                {
                    CommandType = DbCommandType
                };

                // Copy all the parameters to this adapter
                sql_add_params_to_command(sqlCommand, DbParameters);

                // Run the command itself
                try
                {
                    sqlCommand.BeginExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Error executing non-query command." + Environment.NewLine + ex.Message, ex);
                }

                // Return
                return;
            }

            if (DbType == EalDbTypeEnum.PostgreSQL)
            {
                throw new ApplicationException("Support for PostgreSQL with SobekCM is targeted for early 2016");
            }

            throw new ApplicationException("Unknown database type not supported");
        }

        #region Helper methods for the SQL option

        private static void sql_add_params_to_command(SqlCommand SqlCommand, List<EalDbParameter> DbParameters)
        {
            // Copy all the parameters to this adapter
            if ((DbParameters != null) && (DbParameters.Count > 0))
            {
                // Step through each parameter
                foreach (EalDbParameter thisParam in DbParameters)
                {
                    // Determine the appropriate SQL TYPE
                    SqlDbType sqlType = SqlDbType.NVarChar;
                    switch (thisParam.DbType)
                    {
                        case DbType.AnsiString:
                            sqlType = SqlDbType.VarChar;
                            break;

                        case DbType.String:
                            sqlType = SqlDbType.NVarChar;
                            break;

                        case DbType.DateTime:
                            sqlType = SqlDbType.DateTime;
                            break;

                        case DbType.Int16:
                            sqlType = SqlDbType.SmallInt;
                            break;

                        case DbType.Int32:
                            sqlType = SqlDbType.Int;
                            break;

                        case DbType.Int64:
                            sqlType = SqlDbType.BigInt;
                            break;

                        case DbType.Boolean:
                            sqlType = SqlDbType.Bit;
                            break;
                    }


                    // Create the sql parameter
                    SqlParameter sqlParam = new SqlParameter(thisParam.ParameterName, sqlType)
                    {
                        Direction = thisParam.Direction,
                        Value = thisParam.Value
                    };

                    // Add this to the select command
                    SqlCommand.Parameters.Add(sqlParam);
                }
            }
        }

        private static void sql_add_params_to_command(SqlCommand SqlCommand, EalDbParameter[] DbParameters)
        {
            // Copy all the parameters to this adapter
            if ((DbParameters != null) && (DbParameters.Length > 0))
            {
                // Step through each parameter
                foreach (EalDbParameter thisParam in DbParameters)
                {
                    // Determine the appropriate SQL TYPE
                    SqlDbType sqlType = SqlDbType.NVarChar;
                    switch (thisParam.DbType)
                    {
                        case DbType.AnsiString:
                            sqlType = SqlDbType.VarChar;
                            break;

                        case DbType.String:
                            sqlType = SqlDbType.NVarChar;
                            break;

                        case DbType.DateTime:
                            sqlType = SqlDbType.DateTime;
                            break;

                        case DbType.Int16:
                            sqlType = SqlDbType.SmallInt;
                            break;

                        case DbType.Int32:
                            sqlType = SqlDbType.Int;
                            break;

                        case DbType.Int64:
                            sqlType = SqlDbType.BigInt;
                            break;

                        case DbType.Boolean:
                            sqlType = SqlDbType.Bit;
                            break;
                    }


                    // Create the sql parameter
                    SqlParameter sqlParam = new SqlParameter(thisParam.ParameterName, sqlType)
                    {
                        Direction = thisParam.Direction,
                        Value = thisParam.Value
                    };

                    // Add this to the select command
                    SqlCommand.Parameters.Add(sqlParam);
                }
            }
        }

       // Copy any output values back to the parameters
        private static void sql_copy_returned_values_back_to_params(SqlParameterCollection SqlParams, List<EalDbParameter> EalParams)
        {
            // Copy over any values as necessary
            int i = 0;
            foreach (EalDbParameter thisParameter in EalParams)
            {
                if ((thisParameter.Direction == ParameterDirection.Output) || (thisParameter.Direction == ParameterDirection.InputOutput))
                {
                    thisParameter.Value = SqlParams[i].Value;
                }
                i++;
            }
        }

        // Copy any output values back to the parameters
        private static void sql_copy_returned_values_back_to_params(SqlParameterCollection SqlParams, EalDbParameter[] EalParams)
        {
            // Copy over any values as necessary
            int i = 0;
            foreach (EalDbParameter thisParameter in EalParams)
            {
                if ((thisParameter.Direction == ParameterDirection.Output) || (thisParameter.Direction == ParameterDirection.InputOutput))
                {
                    thisParameter.Value = SqlParams[i].Value;
                }
                i++;
            }
        }

        // Copy any output values back to the parameters
        private static void sql_copy_returned_values_back_to_params(EalDbReaderWrapper Wrapper, SqlParameterCollection SqlParams, List<EalDbParameter> EalParams)
        {
            // Copy over any values as necessary
            int i = 0;
            foreach (EalDbParameter thisParameter in EalParams)
            {
                if ((thisParameter.Direction == ParameterDirection.Output) || (thisParameter.Direction == ParameterDirection.InputOutput))
                {
                    Wrapper.Add_Parameter_Copy_Pair(thisParameter, SqlParams[i]);
                }
                i++;
            }
        }

        // Copy any output values back to the parameters
        private static void sql_copy_returned_values_back_to_params(EalDbReaderWrapper Wrapper, SqlParameterCollection SqlParams, EalDbParameter[] EalParams)
        {
            // Copy over any values as necessary
            int i = 0;
            foreach (EalDbParameter thisParameter in EalParams)
            {
                if ((thisParameter.Direction == ParameterDirection.Output) || (thisParameter.Direction == ParameterDirection.InputOutput))
                {
                    Wrapper.Add_Parameter_Copy_Pair(thisParameter, SqlParams[i]);
                }
                i++;
            }
        }

        #endregion
    }
}
