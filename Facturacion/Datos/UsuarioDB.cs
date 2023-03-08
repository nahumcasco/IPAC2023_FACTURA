﻿using Entidades;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;

namespace Datos
{
    public class UsuarioDB
    {
        string cadena = "server=localhost; user=root; database=mifactura; password=123456;";

        public Usuario Autenticar(Login login)
        {
            Usuario user = null;
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * FROM usuario WHERE CodigoUsuario = @CodigoUsuario AND Contrasena = @Contrasena;");

                using (MySqlConnection _conexion = new MySqlConnection(cadena))
                {
                    _conexion.Open();
                    using (MySqlCommand comando = new MySqlCommand(sql.ToString(), _conexion))
                    {
                        comando.CommandType = CommandType.Text;
                        comando.Parameters.Add("@CodigoUsuario", MySqlDbType.VarChar, 50).Value = login.CodigoUsuario;
                        comando.Parameters.Add("@Contrasena", MySqlDbType.VarChar, 80).Value = login.Contraseña;

                        MySqlDataReader dr = comando.ExecuteReader();
                        if (dr.Read())
                        {
                            user = new Usuario();

                            user.CodigoUsuario = dr["CodigoUsuario"].ToString();
                            user.Nombre = dr["Nombre"].ToString();
                            user.Contraseña = dr["Contrasena"].ToString();
                            user.Correo = dr["Correo"].ToString();
                            user.FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"]);
                            user.Rol = dr["Rol"].ToString();
                            if (dr["Foto"].GetType() != typeof(DBNull))
                            {
                                user.Foto = (byte[])dr["Foto"];
                            }
                            user.EstaActivo = Convert.ToBoolean(dr["EstaActivo"]);
                        }
                    }
                }
            }
            catch (System.Exception)
            {
            }
            return user;
        }

        public bool Insertar(Usuario user)
        {
            bool inserto = false;
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" INSERT INTO usuario VALUES ");
                sql.Append(" (@CodigoUsuario, @Nombre, @Contrasena, @Correo, @FechaCreacion, @Rol, @Foto, @EstaActivo);");

                using (MySqlConnection _conexion = new MySqlConnection(cadena))
                {
                    _conexion.Open();
                    using (MySqlCommand comando = new MySqlCommand(sql.ToString(), _conexion))
                    {
                        comando.CommandType = CommandType.Text;
                        comando.Parameters.Add("@CodigoUsuario", MySqlDbType.VarChar, 50).Value = user.CodigoUsuario;
                        comando.Parameters.Add("@Nombre", MySqlDbType.VarChar, 45).Value = user.Nombre;
                        comando.Parameters.Add("@Contrasena", MySqlDbType.VarChar, 80).Value = user.Contraseña;
                        comando.Parameters.Add("@Correo", MySqlDbType.VarChar, 45).Value = user.Correo;
                        comando.Parameters.Add("@FechaCreacion", MySqlDbType.DateTime).Value = DateTime.Now;
                        comando.Parameters.Add("@Rol", MySqlDbType.VarChar, 20).Value = user.Rol;
                        comando.Parameters.Add("@Foto", MySqlDbType.LongBlob).Value = user.Foto;
                        comando.Parameters.Add("@EstaActivo", MySqlDbType.Bit).Value = user.EstaActivo;
                        comando.ExecuteNonQuery();
                        inserto = true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return inserto;
        }

        public bool Editar(Usuario user)
        {
            bool edito = false;
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" UPDATE usuario SET ");
                sql.Append(" Nombre = @Nombre, Contrasena = @Contrasena, Correo = @Correo, Rol = @Rol, Foto = @Foto, EstaActivo = @EstaActivo");
                sql.Append(" WHERE CodigoUsuario = @CodigoUsuario;");
                using (MySqlConnection _conexion = new MySqlConnection(cadena))
                {
                    _conexion.Open();
                    using (MySqlCommand comando = new MySqlCommand(sql.ToString(), _conexion))
                    {
                        comando.CommandType = CommandType.Text;
                        comando.Parameters.Add("@CodigoUsuario", MySqlDbType.VarChar, 50).Value = user.CodigoUsuario;
                        comando.Parameters.Add("@Nombre", MySqlDbType.VarChar, 45).Value = user.Nombre;
                        comando.Parameters.Add("@Contrasena", MySqlDbType.VarChar, 80).Value = user.Contraseña;
                        comando.Parameters.Add("@Correo", MySqlDbType.VarChar, 45).Value = user.Correo;
                        comando.Parameters.Add("@FechaCreacion", MySqlDbType.DateTime).Value = DateTime.Now;
                        comando.Parameters.Add("@Rol", MySqlDbType.VarChar, 20).Value = user.Rol;
                        comando.Parameters.Add("@Foto", MySqlDbType.LongBlob).Value = user.Foto;
                        comando.Parameters.Add("@EstaActivo", MySqlDbType.Bit).Value = user.EstaActivo;
                        comando.ExecuteNonQuery();
                        edito = true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return edito;
        }

        public bool Eliminar(string codigoUsuario)
        {
            bool elimino = false;
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" DELETE FROM usuario ");
                sql.Append(" WHERE CodigoUsuario = @CodigoUsuario;");
                using (MySqlConnection _conexion = new MySqlConnection(cadena))
                {
                    _conexion.Open();
                    using (MySqlCommand comando = new MySqlCommand(sql.ToString(), _conexion))
                    {
                        comando.CommandType = CommandType.Text;
                        comando.Parameters.Add("@CodigoUsuario", MySqlDbType.VarChar, 50).Value = codigoUsuario;
                        comando.ExecuteNonQuery();
                        elimino = true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return elimino;
        }

        public DataTable DevolverUsuarios()
        {
            DataTable dataTable = new DataTable();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" SELECT * FROM usuario ");
                using (MySqlConnection _conexion = new MySqlConnection(cadena))
                {
                    _conexion.Open();
                    using (MySqlCommand comando = new MySqlCommand(sql.ToString(), _conexion))
                    {
                        comando.CommandType = CommandType.Text;
                        MySqlDataReader dr = comando.ExecuteReader();
                        dataTable.Load(dr);
                    }
                }
            }
            catch (Exception)
            {
            }
            return dataTable;
        }

        public byte[] DevolverImagen(string codigoUsuario)
        {
            byte[] imagen = new byte[0];
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" SELECT Foto FROM usuario WHERE CodigoUsuario = @CodigoUsuario; ");
                using (MySqlConnection _conexion = new MySqlConnection(cadena))
                {
                    _conexion.Open();
                    using (MySqlCommand comando = new MySqlCommand(sql.ToString(), _conexion))
                    {
                        comando.CommandType = CommandType.Text;
                        comando.Parameters.Add("@CodigoUsuario", MySqlDbType.VarChar, 50).Value = codigoUsuario;
                        MySqlDataReader dr = comando.ExecuteReader();
                        if (dr.Read())
                        {
                            imagen = (byte[])dr["Foto"];
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return imagen;
        }
    }
}
