using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http;
using TagLib;

public class CancionService
{
    public CancionService() { }

    public int ObtenerDuracionCancion(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            throw new ArgumentException("Archivo inválido");

        using var stream = archivo.OpenReadStream();

        var tagFile = TagLib.File.Create(new StreamFileAbstraction(archivo.FileName, stream, stream));
        return (int)Math.Round(tagFile.Properties.Duration.TotalSeconds);
    }

    // Clase privada anidada para adaptar el stream a TagLib
    private class StreamFileAbstraction : TagLib.File.IFileAbstraction
    {
        private readonly Stream _readStream;
        private readonly Stream _writeStream;

        public StreamFileAbstraction(string name, Stream readStream, Stream writeStream)
        {
            Name = name;
            _readStream = readStream;
            _writeStream = writeStream;
        }

        public string Name { get; }

        public Stream ReadStream => _readStream;

        public Stream WriteStream => _writeStream;

        public void CloseStream(Stream stream)
        {
            // No cerramos el stream aquí para evitar problemas con el stream original
        }
    }
}
