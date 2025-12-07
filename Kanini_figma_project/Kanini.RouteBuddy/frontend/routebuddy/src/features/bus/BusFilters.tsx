import { useState } from 'react';
import {
  FormGroup,
  FormControlLabel,
  Checkbox,
  Slider,
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
    <div className="filters-card">
      <div className="filters-header">
        <h3 className="filters-title">Filters</h3>
        <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
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
        </div>
      </div>

      {/* Sort By */}
      <div className="filter-section">
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
      </div>

      {/* Bus Types */}
      <div className="filter-section">
        <div className="filter-section-title">Bus Type</div>
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
      </div>

      {/* Amenities */}
      <div className="filter-section">
        <div className="filter-section-title">Amenities</div>
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
      </div>

      {/* Price Range */}
      <div className="filter-section">
        <div className="filter-section-title">
          Price Range: ₹{localFilters.priceRange[0]} - ₹{localFilters.priceRange[1]}
        </div>
        <Slider
          value={localFilters.priceRange}
          onChange={handlePriceChange}
          valueLabelDisplay="auto"
          min={0}
          max={getMaxPrice()}
          step={50}
          valueLabelFormat={(value) => `₹${value}`}
        />
      </div>
    </div>
  );
};

export default BusFilters;