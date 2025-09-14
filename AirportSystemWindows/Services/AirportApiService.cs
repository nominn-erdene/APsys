using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AirportSystemWindows.Services
{
    /// <summary>
    /// Аэропортын API-тай харилцах service класс.
    /// Нислэг, зорчигч, суудал зэрэг мэдээллийг API-аас авна.
    /// </summary>
    public class AirportApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        /// <summary>
        /// AirportApiService-ийн конструктор.
        /// HTTP client болон API-ийн суурь URL-ийг тохируулна.
        /// </summary>
        public AirportApiService()
        {
            _httpClient = new HttpClient();
            _baseUrl = "http://10.3.202.148:5000/api";
        }

        /// <summary>
        /// Бүх нислэгийн мэдээллийг API-аас авна.
        /// </summary>
        /// <returns>Нислэгийн жагсаалт.</returns>
        public async Task<List<FlightApiResponse>> GetFlightsAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_baseUrl}/flights");
                return JsonSerializer.Deserialize(response, AppJsonSerializerContext.Default.ListFlightApiResponse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch flights: {ex.Message}");
            }
        }

        /// <summary>
        /// Паспортын дугаараар зорчигчийн мэдээллийг API-аас авна.
        /// </summary>
        /// <param name="passportNumber">Зорчигчийн паспортын дугаар.</param>
        /// <returns>Зорчигчийн мэдээлэл.</returns>
        public async Task<PassengerApiResponse> GetPassengerByPassportAsync(string passportNumber)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_baseUrl}/passengers?passport={passportNumber}");
                var passengers = JsonSerializer.Deserialize(response, AppJsonSerializerContext.Default.ListPassengerApiResponse);
                return passengers?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch passenger: {ex.Message}");
            }
        }

        /// <summary>
        /// Зорчигчийг check-in хийх API дуудалт.
        /// </summary>
        /// <param name="passportNumber">Зорчигчийн паспортын дугаар.</param>
        /// <param name="selectedSeatNumber">Сонгосон суудлын дугаар (сонголттой).</param>
        /// <returns>Check-in үйлдлийн үр дүн.</returns>
        public async Task<CheckInApiResponse> CheckInPassengerAsync(string passportNumber, string? selectedSeatNumber = null)
        {
            try
            {
                // This is the fix: Use a real, named class to avoid trimming issues.
                var request = new CheckInRequest
                {
                    PassportNumber = passportNumber,
                    SelectedSeatNumber = selectedSeatNumber
                };

                // Use the source generator to serialize the new request type.
                var json = JsonSerializer.Serialize(request, AppJsonSerializerContext.Default.CheckInRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/checkin", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize(responseContent, AppJsonSerializerContext.Default.CheckInApiResponse);
                }
                else
                {
                    var errorResponse = JsonSerializer.Deserialize(responseContent, AppJsonSerializerContext.Default.ErrorResponse);
                    throw new Exception(errorResponse?.Message ?? "Check-in failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check-in passenger: {ex.Message}");
            }
        }

        /// <summary>
        /// Нислэгийн статусыг шинэчлэх API дуудалт.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID.</param>
        /// <param name="status">Шинэ статус.</param>
        /// <returns>Үйлдэл амжилттай болсон эсэх.</returns>
        public async Task<bool> UpdateFlightStatusAsync(int flightId, string status)
        {
            try
            {
                var request = new { flightStatus = status };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}/flights/{flightId}/status", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex) { throw new Exception($"Failed to update flight status: {ex.Message}"); }
        }

        /// <summary>
        /// Тодорхой нислэгийн суудлын мэдээллийг API-аас авна.
        /// </summary>
        /// <param name="flightId">Нислэгийн ID.</param>
        /// <returns>Суудлын жагсаалт.</returns>
        public async Task<List<SeatApiResponse>> GetSeatsByFlightAsync(int flightId)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_baseUrl}/seats/flight/{flightId}");
                return JsonSerializer.Deserialize(response, AppJsonSerializerContext.Default.ListSeatApiResponse);
            }
            catch (Exception ex) { throw new Exception($"Failed to fetch seats: {ex.Message}"); }
        }

        /// <summary>
        /// HTTP client-ийг устгах арга.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    // --- Data Transfer Objects (DTOs) ---

    /// <summary>
    /// Check-in хүсэлтийн мэдээлэл.
    /// </summary>
    public class CheckInRequest
    {
        /// <summary>
        /// Зорчигчийн паспортын дугаар.
        /// </summary>
        public string PassportNumber { get; set; }
        
        /// <summary>
        /// Сонгосон суудлын дугаар (сонголттой).
        /// </summary>
        public string? SelectedSeatNumber { get; set; }
    }

    /// <summary>
    /// API-аас ирсэн нислэгийн мэдээллийн хариу.
    /// </summary>
    public class FlightApiResponse
    {
        /// <summary>
        /// FlightApiResponse-ийн конструктор.
        /// </summary>
        public FlightApiResponse() { }
        
        /// <summary>
        /// Нислэгийн ID.
        /// </summary>
        public int FlightID { get; set; }
        
        /// <summary>
        /// Нислэгийн дугаар.
        /// </summary>
        public string FlightNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Хөөрөх аэропорт.
        /// </summary>
        public string ArrivalAirport { get; set; } = string.Empty;
        
        /// <summary>
        /// Буух аэропорт.
        /// </summary>
        public string DestinationAirport { get; set; } = string.Empty;
        
        /// <summary>
        /// Нислэгийн цаг.
        /// </summary>
        public DateTime Time { get; set; }
        
        /// <summary>
        /// Хаалганы дугаар.
        /// </summary>
        public string Gate { get; set; } = string.Empty;
        
        /// <summary>
        /// Нислэгийн статус (тоон утга).
        /// </summary>
        public int FlightStatus { get; set; }
        
        /// <summary>
        /// Суудлын жагсаалт.
        /// </summary>
        public List<SeatApiResponse> Seats { get; set; } = new List<SeatApiResponse>();
        
        /// <summary>
        /// Зорчигчдын жагсаалт.
        /// </summary>
        public List<PassengerApiResponse> Passengers { get; set; } = new List<PassengerApiResponse>();
    }

    /// <summary>
    /// API-аас ирсэн зорчигчийн мэдээллийн хариу.
    /// </summary>
    public class PassengerApiResponse
    {
        /// <summary>
        /// PassengerApiResponse-ийн конструктор.
        /// </summary>
        public PassengerApiResponse() { }
        
        /// <summary>
        /// Зорчигчийн ID.
        /// </summary>
        public int PassengerID { get; set; }
        
        /// <summary>
        /// Зорчигчийн бүтэн нэр.
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        
        /// <summary>
        /// Паспортын дугаар.
        /// </summary>
        public string PassportNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Нислэгийн ID.
        /// </summary>
        public int FlightID { get; set; }
        
        /// <summary>
        /// Томилогдсон суудлын ID.
        /// </summary>
        public int? AssignedSeatID { get; set; }
        
        /// <summary>
        /// Check-in хийсэн эсэх.
        /// </summary>
        public bool IsCheckedIn { get; set; }
        
        /// <summary>
        /// Нислэгийн мэдээлэл.
        /// </summary>
        public FlightApiResponse? Flight { get; set; }
        
        /// <summary>
        /// Томилогдсон суудлын мэдээлэл.
        /// </summary>
        public SeatApiResponse? AssignedSeat { get; set; }
    }

    /// <summary>
    /// API-аас ирсэн суудлын мэдээллийн хариу.
    /// </summary>
    public class SeatApiResponse
    {
        /// <summary>
        /// SeatApiResponse-ийн конструктор.
        /// </summary>
        public SeatApiResponse() { }
        
        /// <summary>
        /// Суудлын ID.
        /// </summary>
        public int SeatID { get; set; }
        
        /// <summary>
        /// Нислэгийн ID.
        /// </summary>
        public int FlightID { get; set; }
        
        /// <summary>
        /// Суудлын дугаар.
        /// </summary>
        public string SeatNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Суудал эзэлсэн эсэх.
        /// </summary>
        public bool IsOccupied { get; set; }
        
        /// <summary>
        /// Эзэмшигч зорчигчийн ID.
        /// </summary>
        public int? PassengerID { get; set; }
        
        /// <summary>
        /// Нислэгийн мэдээлэл.
        /// </summary>
        public FlightApiResponse? Flight { get; set; }
        
        /// <summary>
        /// Эзэмшигч зорчигчийн мэдээлэл.
        /// </summary>
        public PassengerApiResponse? Passenger { get; set; }
    }

    /// <summary>
    /// Check-in үйлдлийн хариу.
    /// </summary>
    public class CheckInApiResponse
    {
        /// <summary>
        /// CheckInApiResponse-ийн конструктор.
        /// </summary>
        public CheckInApiResponse() { }
        
        /// <summary>
        /// Үйлдэл амжилттай болсон эсэх.
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Мессеж.
        /// </summary>
        public string Message { get; set; } = string.Empty;
        
        /// <summary>
        /// Зорчигчийн нэр.
        /// </summary>
        public string PassengerName { get; set; } = string.Empty;
        
        /// <summary>
        /// Нислэгийн дугаар.
        /// </summary>
        public string FlightNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Суудлын дугаар.
        /// </summary>
        public string SeatNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Хаалганы дугаар.
        /// </summary>
        public string Gate { get; set; } = string.Empty;
    }

    /// <summary>
    /// Алдааны мэдээллийн хариу.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// ErrorResponse-ийн конструктор.
        /// </summary>
        public ErrorResponse() { }
        
        /// <summary>
        /// Алдааны мессеж.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}