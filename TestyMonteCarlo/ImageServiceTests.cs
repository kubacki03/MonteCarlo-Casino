using System;

using Moq;
using Xunit;
using MonteCarlo.NET.Services;

public class ImageServiceTests
{
    [Fact]
    public void CheckAge_ReturnsTrue_ForValidAdultDate()
    {
        
        string mockText = "C:\\Users\\Kuba\\source\\repos\\montecarlo.net\\MonteCarlo.NET\\wwwroot\\uploads\\71d5c0e5-b6b1-4d17-9425-55062f568a5d.png";


     
        var result = ImageService.CheckAge(mockText);

    }

    [Fact]
    public void CheckAge_ReturnsFalse_ForInvalidAdultDate()
    {

        string mockText = "C:\\Users\\Kuba\\source\\repos\\montecarlo.net\\MonteCarlo.NET\\wwwroot\\uploads\\niepelno.png";



        var result = ImageService.CheckAge(mockText);

    }
}
