# R U Nearby

This project looks at handling postcode/ and location data, primarily UK focussed, but with some globally applicable methods.

# Conventions

- Where converted, map points will be referenced as Latitude (Y axis, North-South)/Longitude (X axis, East-West) in line with this [article](https://stackoverflow.com/questions/18636564/lat-long-or-long-lat).
- Also, Google use Lat Long, so it will make that lookup easier.


Initial data for UK does not include NI.   

# Functions

## Global -> VanillaWebPages / gmaps-draw-circles.html

Fully interactive Google Map, where you can set a starting location + a radius via click or data input. You can then edit that information.

This will enable someone to set a location and a radius around that location. The data is output to plain JavaScript variables. No jQuery or Frameworks used :innocent: . 

You must add your own Google Maps API Key in 2 places.

This page also includes Google Geocode, which can convert a postcode/ zip code/ address into coordinates.

## Global -> GlobalVendorRadiusCheck.cs -> GetVendorsWhoCanVisitCustomer

Assumes vendors have marked their location and a radius they are willing to travel using the gmaps-draw-circles.html. This will take a persons co-ordinates and see which vendors could visit. This does not have island etc exclusions as it assumes the vendor would have marked the radius as they want it.

This example uses dummy UK data, but is interfaced and DI'd so could be swapped out for something else.

## Global -> GlobalVendorRadiusCheck.cs -> GetVendorsCustomerCanVisit

Assumes a customer has given their coordinates and a radius they are willing to travel. This will look for all vendors within the radius the customer has specified.

This example uses dummy UK data, but is interfaced and DI'd so could be swapped out for something else.

## UK -> FindPostcodesInRadiusKm

Given a postcode, what other postcodes are within a specified radius (options to exclude Islands and Crossing the Briston Channel / River Severn).

## UK -> GetClosestPostcodeToCoordinate

Given a set of coordinates, what is the nearest postcode?


# Process For Setting Up From Fresh OS Map Data.

1. Get [CodePointOpen](https://osdatahub.os.uk/downloads/open/CodePointOpen) data and put it in {root}\Demo\CoreLookup\Data\UK\CodePointOpen2020\Data\CSV. There will be a `.csv` file for each postcode with Eastings/Northings data.

2. `IOSMapConverter` has a method ***`Convert_OS_CodePointOpen_Data_To_Usable_LatLong_Data()`*** which is currently called from CoreLookup\Main.cs (commented out). This will do the following, which will be output to CoreLookup\Data\UK\CodePointOpen2020\Data_Processed_LatLong:

- Convert each postcode file to a new file with data: Postcode|lat|long
- Merge all the csv files to one big file of all postcodes (see 'Merged\uk_postcodes_lat_long_full.csv').
- Take the 'Merged' data and simplify this down to 'Area + District' data (e.g. PO4 8RA -> PO4). It will take the average coordinates of all the same 'Area+District' postcodes to provide new data e.g. "PO4",50.790176,-1.062084 (see 'Merged\uk_postcodes_lat_long_area_district_only.csv').


## Testing

The project contains unit tests using xUnit and Moq.
