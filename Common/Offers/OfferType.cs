using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessRulesMigrator.Common.Offers
{
    internal static class Offer
    {
        public static readonly Dictionary<int, string> OfferCategories = new Dictionary<int, string>
        {
            { 1, "Data" },
            { 2, "Video" },
            { 3, "Voice" },
            { 4, "Bundle" },
            { 5, "Content" },
            { 6, "Hardware" },
            { 7, "HomeServices" },
            { 8, "FinancialService" },
            { 9, "PrintSubscription" },
            { 10, "CommunicationDevice" },
            { 11, "Computers" },
            { 12, "ConsumerElectronics" },
            { 13, "DigitalSubscription" },
            { 14, "Energy" },
            { 15, "SupportService" },
        };
        public static readonly Dictionary<int, (int Category, string Type)> OfferTypes = new Dictionary<int, (int Category, string Type)>
        {
            { 1, (1, "Cable") },
            { 2, (1, "DSL") },
            { 3, (1, "Fiber") },
            { 4, (1, "Standalone") },
            { 5, (1, "Satellite") },
            { 6, (1, "WiMaxx") },
            { 7, (1, "DialUp") },
            { 8, (2, "AnalogCable") },
            { 9, (2, "DigitalCable") },
            { 10, (2, "Satellite") },
            { 11, (2, "Fiber") },
            { 12, (2, "HD") },
            { 13, (2, "DVR") },
            { 14, (2, "HDDVR") },
            { 15, (3, "VOIP") },
            { 16, (3, "Cable") },
            { 17, (3, "POTS") },
            { 18, (3, "LongDistance") },
            { 19, (4, "DoublePlay") },
            { 20, (4, "TriplePlay") },
            { 21, (4, "QuadPlay") },
            { 22, (5, "Content") },
            { 23, (6, "MobileWirelessCard") },
            { 24, (6, "Hardware") },
            { 25, (7, "ProfessionalInstallation") },
            { 26, (7, "HomeSecurity") },
            { 27, (7, "SurgeProtection") },
            { 28, (7, "Warranty") },
            { 29, (7, "Mover") },
            { 30, (8, "HomeInsurance") },
            { 31, (8, "LifeInsurance") },
            { 32, (8, "CreditCard") },
            { 33, (8, "AssetManagement") },
            { 34, (9, "Magazine") },
            { 35, (9, "Newspaper") },
            { 36, (10, "SmartPhone") },
            { 37, (10, "MobilePhone") },
            { 38, (10, "HomePhone") },
            { 39, (10, "PDA") },
            { 40, (11, "NotebookPC") },
            { 41, (11, "DesktopPC") },
            { 42, (11, "Netbook") },
            { 43, (11, "Printer") },
            { 44, (11, "Router") },
            { 45, (12, "GPS") },
            { 46, (12, "TV") },
            { 47, (12, "DVR") },
            { 48, (12, "TVDevice") },
            { 49, (13, "Music") },
            { 50, (13, "Security") },
            { 51, (13, "Books") },
            { 52, (13, "Software") },
            { 53, (13, "Entertainment") },
            { 54, (14, "Electricity") },
            { 55, (14, "Gas") },
            { 56, (14, "Alternative") },
            { 57, (15, "SupportService") },
            { 58, (7, "RentersInsurance") },
            { 59, (7, "CarInsurance") },
            { 60, (7, "WaterService") },
            { 61, (7, "PestControl") },
            { 62, (7, "WaterHeater") },
            { 63, (7, "SurgeCoverageGrounding") },
            { 64, (7, "HeatingCoolingRepair") },
            { 65, (7, "PlumbingRepair") },
            { 70, (7, "KitchenRepair") },
            { 71, (7, "LaundryRepair") },
            { 72, (2, "Streaming") },
            { 73, (7, "MovingServices") },
            { 74, (6, "HardwareInstallation") },

        };
        internal static string GetOfferCategoryForOfferType(int offerTypeId)
        {
            return OfferTypes.ContainsKey(offerTypeId) ? OfferCategories[OfferTypes[offerTypeId].Category] : "";
        }
    }
    
}
