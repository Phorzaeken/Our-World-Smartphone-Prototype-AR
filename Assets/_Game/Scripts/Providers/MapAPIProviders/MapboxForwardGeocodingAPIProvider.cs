using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Mapbox.CheapRulerCs;
using MBForwardGeocodeResponse = Mapbox.Geocoding.ForwardGeocodeResponse;
using OurWorld.Scripts.DataModels;
using OurWorld.Scripts.DataModels.GeolocationData;
using OurWorld.Scripts.DataModels.MapAPIResponses;
using OurWorld.Scripts.Extensions;
using OurWorld.Scripts.Helpers.DataMappers;
using OurWorld.Scripts.Interfaces;
using OurWorld.Scripts.Interfaces.MapAPI.Geocoding;
using Mapbox.Geocoding;
using OurWorld.Scripts.DataModels.MapAPIRequests;

namespace OurWorld.Scripts.Providers.MapAPIProviders
{
    public class MapboxForwardGeocodingAPIProvider : IForwardGeocodingProvider
    {
        private static string APIToken = MapboxAPIProvider.ApiToken;
        private static string BaseAPIUrl = MapboxAPIProvider.BaseAPIUrl;

        private IWebRequestHelper _webRequestHelper;

        public MapboxForwardGeocodingAPIProvider(IWebRequestHelper webRequestHelper)
        {
            _webRequestHelper = webRequestHelper;
        }

        public async UniTask<ForwardGeocodingResponse<POIData>> POISearchAsync(IRequest request)
        {
            var forwardGeocodeRequest = (MapBoxForwardGeocodingRequest)request;

            UriBuilder uriBuilder = new UriBuilder($"{BaseAPIUrl}{forwardGeocodeRequest.GetRequestURLParameters()}");

            var tokenQueryParameter = $"access_token={APIToken}";

            if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
                uriBuilder.Query = $"{uriBuilder.Query.Substring(1)}&{tokenQueryParameter}";
            else
                uriBuilder.Query = $"?{tokenQueryParameter}";

            var result = await _webRequestHelper.GetAsync<MBForwardGeocodeResponse>(uriBuilder.Uri);

            var forwardGeocodeResponse = new ForwardGeocodingResponse<POIData>(result.Success, PopulatePOIDataFromFeatures(result.Data.Features, forwardGeocodeRequest.Proximity));

            return forwardGeocodeResponse;
        }

        #region Helpers
        private List<POIData> PopulatePOIDataFromFeatures(List<Feature> features, Geolocation playerLocation)
        {
            IDataMapper<Feature, POIData> poiDataMapper = new POIDataMapper();

            var ruler = new CheapRuler(playerLocation.Latitude, CheapRulerUnits.Kilometers);

            List<POIData> poiDataList = new List<POIData>();

            foreach (var feature in features)
            {
                var poiData = poiDataMapper.MapObject(feature);
                poiData.Distance = (float)ruler.Distance(playerLocation.ToLonLatArray(), poiData.Geolocation.ToLonLatArray());
                poiDataList.Add(poiData);
            }

            return poiDataList;
        }
        #endregion
    }
}