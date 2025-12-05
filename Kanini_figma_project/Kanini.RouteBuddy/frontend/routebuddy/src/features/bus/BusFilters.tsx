import { useState } from 'react';
import {
  Card,
  CardContent,
  Typography,
  FormGroup,
  FormControlLabel,
  Checkbox,
  Slider,
  Box,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Button,
  Chip,
} from '@mui/material';
import { useAppDispatch, useAppSelector } from '../../hooks/useAppDispatch';
import { updateFilters, resetFilters } from './busSlice';
import { BusType, BusAmenities } from '../../utils/constants';

const BusFilters = () => {
  const dispatch = useAppDispatch();
  const { filters, searchResults } = useAppSelector((state) => state.bus);
  
  const [localFilters, setLocalFilters] = useState(filters);

  const busTypeOptions = [
    { value: BusType.AC, label: 'AC' },
    { value: BusType.NonAC, label: 'Non-AC' },
    { value: BusType.Sleeper, label: 'Sleeper' },
    { value: BusType.SemiSleeper, label: 'Semi-Sleeper' },
    { value: BusType.Volvo, label: 'Volvo' },
    { value: BusType.Luxury, label: 'Luxury' },
  ];

  const amenityOptions = [
    { value: BusAmenities.AC, label: 'AC' },
    { value: BusAmenities.WiFi, label: 'WiFi' },
    { value: BusAmenities.Charging, label: 'Charging' },
    { value: BusAmenities.Blanket, label: 'Blanket' },
    { value: BusAmenities.Pillow, label: 'Pillow' },
    { value: BusAmenities.Entertainment, label: 'Entertainment' },
    { value: BusAmenities.Snacks, label: 'Snacks' },
    { value: BusAmenities.WashRoom, label: 'Washroom' },
  ];

  const sortOptions = [
    { value: 'price', label: 'Price (Low to High)' },
    { value: 'departure', label: 'Departure Time' },
    { value: 'duration', label: 'Duration' },
  ];

  const handleBusTypeChange = (busType: number, checked: boolean) => {
    const newBusTypes = checked
      ? [...localFilters.busTypes, busType]
      : localFilters.busTypes.filter(type => type !== busType);
    
    const newFilters = { ...localFilters, busTypes: newBusTypes };
    setLocalFilters(newFilters);
    dispatch(updateFilters(newFilters));
  };

  const handleAmenityChange = (amenity: number, checked: boolean) => {
    const newAmenities = checked
      ? [...localFilters.amenities, amenity]
      : localFilters.amenities.filter(a => a !== amenity);
    
    const newFilters = { ...localFilters, amenities: newAmenities };
    setLocalFilters(newFilters);
    dispatch(updateFilters(newFilters));
  };

  const handlePriceChange = (event: Event, newValue: number | number[]) => {
    const priceRange = newValue as [number, number];
    const newFilters = { ...localFilters, priceRange };
    setLocalFilters(newFilters);
    dispatch(updateFilters(newFilters));
  };

  const handleSortChange = (sortBy: string) => {
    const newFilters = { ...localFilters, sortBy };
    setLocalFilters(newFilters);
    dispatch(updateFilters(newFilters));
  };

  const handleReset = () => {
    setLocalFilters({
      busTypes: [],
      amenities: [],
      priceRange: [0, 5000],
      departureTime: ['00:00', '23:59'],
      sortBy: 'price',
    });
    dispatch(resetFilters());
  };

  const getMaxPrice = () => {
    if (searchResults.length === 0) return 5000;
    return Math.max(...searchResults.map(bus => bus.basePrice));
  };

  const getActiveFiltersCount = () => {
    return localFilters.busTypes.length + localFilters.amenities.length;
  };

  return (
    <Card sx={{ mb: 3 }}>
      <CardContent>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            Filters
          </Typography>
          <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
            {getActiveFiltersCount() > 0 && (
              <Chip 
                label={`${getActiveFiltersCount()} active`} 
                size="small" 
                color="primary" 
              />
            )}
            <Button size="small" onClick={handleReset}>
              Clear All
            </Button>
          </Box>
        </Box>

        {/* Sort By */}
        <Box sx={{ mb: 3 }}>
          <FormControl fullWidth size="small">
            <InputLabel>Sort By</InputLabel>
            <Select
              value={localFilters.sortBy}
              label="Sort By"
              onChange={(e) => handleSortChange(e.target.value)}
            >
              {sortOptions.map((option) => (
                <MenuItem key={option.value} value={option.value}>
                  {option.label}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Box>

        {/* Bus Types */}
        <Box sx={{ mb: 3 }}>
          <Typography variant="subtitle2" sx={{ mb: 1, fontWeight: 600 }}>
            Bus Type
          </Typography>
          <FormGroup row>
            {busTypeOptions.map((option) => (
              <FormControlLabel
                key={option.value}
                control={
                  <Checkbox
                    checked={localFilters.busTypes.includes(option.value)}
                    onChange={(e) => handleBusTypeChange(option.value, e.target.checked)}
                    size="small"
                  />
                }
                label={option.label}
              />
            ))}
          </FormGroup>
        </Box>

        {/* Amenities */}
        <Box sx={{ mb: 3 }}>
          <Typography variant="subtitle2" sx={{ mb: 1, fontWeight: 600 }}>
            Amenities
          </Typography>
          <FormGroup row>
            {amenityOptions.map((option) => (
              <FormControlLabel
                key={option.value}
                control={
                  <Checkbox
                    checked={localFilters.amenities.includes(option.value)}
                    onChange={(e) => handleAmenityChange(option.value, e.target.checked)}
                    size="small"
                  />
                }
                label={option.label}
              />
            ))}
          </FormGroup>
        </Box>

        {/* Price Range */}
        <Box sx={{ mb: 2 }}>
          <Typography variant="subtitle2" sx={{ mb: 2, fontWeight: 600 }}>
            Price Range: ₹{localFilters.priceRange[0]} - ₹{localFilters.priceRange[1]}
          </Typography>
          <Slider
            value={localFilters.priceRange}
            onChange={handlePriceChange}
            valueLabelDisplay="auto"
            min={0}
            max={getMaxPrice()}
            step={50}
            valueLabelFormat={(value) => `₹${value}`}
          />
        </Box>
      </CardContent>
    </Card>
  );
};

export default BusFilters;