namespace Plainer.Endpoints;

public static class PlainerEndPoints
{
    public static WebApplication MapPlainerEndpoints(this WebApplication app){
        app.MapPlainerEventEndpoints();

        app.MapPlainerUserEndpoints();

        return app;
    }
}
