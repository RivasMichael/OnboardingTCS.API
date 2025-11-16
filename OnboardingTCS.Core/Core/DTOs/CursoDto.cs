using System;
using System.Collections.Generic;

namespace OnboardingTCS.Core.Core.DTOs
{
    public class CursoCreateDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public bool Recomendado { get; set; }
        public List<string> RecomendadoPara { get; set; } = new List<string>();
    }

    public class CursoListDto
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public int Inscritos { get; set; }
    }

    /// <summary>
    /// DTO simple con solo los campos esenciales para el frontend
    /// </summary>
    public class CursoSimpleDto
    {
        public string Nivel { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO completo para mostrar cursos con toda la información necesaria en el frontend
    /// </summary>
    public class CursoCompletDto
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public bool Recomendado { get; set; }
        public List<string> RecomendadoPara { get; set; } = new List<string>();
        public int Inscritos { get; set; }
        public bool Activo { get; set; }
    }

    public class CursoDetailDto
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public bool Recomendado { get; set; }
        public List<string> RecomendadoPara { get; set; } = new List<string>();
        public string CreadoPor { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int Inscritos { get; set; }
    }

    public class CursoUpdateDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public bool Recomendado { get; set; }
        public List<string> RecomendadoPara { get; set; } = new List<string>();
        public bool Activo { get; set; }
    }
}