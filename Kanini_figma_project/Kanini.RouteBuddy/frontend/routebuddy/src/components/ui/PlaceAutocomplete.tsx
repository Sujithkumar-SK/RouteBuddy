import { useState, useEffect } from 'react';
import { Autocomplete, TextField, CircularProgress } from '@mui/material';
import api from '../../services/api';

interface Place {
  stopId: number;
  name: string;
  landmark?: string;
}

interface PlaceAutocompleteProps {
  label: string;
  value: string;
  onChange: (value: string) => void;
  onError: (error: string) => void;
  error?: string;
  placeholder?: string;
  required?: boolean;
}

const PlaceAutocomplete = ({
  label,
  value,
  onChange,
  onError,
  error,
  placeholder,
  required = false,
}: PlaceAutocompleteProps) => {
  const [allPlaces, setAllPlaces] = useState<Place[]>([]);
  const [filteredOptions, setFilteredOptions] = useState<Place[]>([]);
  const [loading, setLoading] = useState(false);
  const [inputValue, setInputValue] = useState(value);

  // Fetch all places once on component mount
  useEffect(() => {
    const fetchAllPlaces = async () => {
      setLoading(true);
      try {
        const response = await api.get('/Stop/places');
        setAllPlaces(response.data || []);
        onError('');
      } catch (error: any) {
        console.error('Failed to fetch places:', error);
        onError('Failed to load places');
      } finally {
        setLoading(false);
      }
    };
    
    fetchAllPlaces();
  }, []);

  // Filter places based on input
  useEffect(() => {
    if (!inputValue || inputValue.length < 1) {
      setFilteredOptions([]);
      return;
    }

    const filtered = allPlaces
      .filter(place => 
        place.name.toLowerCase().includes(inputValue.toLowerCase())
      )
      .sort((a, b) => {
        const aStartsWith = a.name.toLowerCase().startsWith(inputValue.toLowerCase());
        const bStartsWith = b.name.toLowerCase().startsWith(inputValue.toLowerCase());
        if (aStartsWith && !bStartsWith) return -1;
        if (!aStartsWith && bStartsWith) return 1;
        return a.name.localeCompare(b.name);
      })
      .slice(0, 10);
    
    setFilteredOptions(filtered);
  }, [inputValue, allPlaces]);

  const handleInputChange = (event: any, newInputValue: string) => {
    setInputValue(newInputValue);
  };

  const handleChange = (event: any, newValue: Place | string | null) => {
    if (typeof newValue === 'string') {
      onChange(newValue);
      setInputValue(newValue);
    } else if (newValue) {
      onChange(newValue.name);
      setInputValue(newValue.name);
    } else {
      onChange('');
      setInputValue('');
    }
  };

  const handleBlur = () => {
    if (inputValue && !allPlaces.some(place => place.name.toLowerCase() === inputValue.toLowerCase())) {
      onError('Invalid place name. Please select from suggestions.');
    }
  };

  return (
    <Autocomplete
      freeSolo
      options={filteredOptions}
      getOptionLabel={(option) => {
        if (typeof option === 'string') return option;
        return option.name;
      }}
      renderOption={(props, option) => (
        <li {...props} key={option.stopId}>
          {option.name}
        </li>
      )}
      inputValue={inputValue}
      onInputChange={handleInputChange}
      onChange={handleChange}
      loading={loading}
      loadingText="Loading places..."
      noOptionsText={inputValue.length < 1 ? "Type to search places" : "No places found"}
      renderInput={(params) => (
        <TextField
          {...params}
          label={label}
          placeholder={placeholder}
          required={required}
          error={!!error}
          helperText={error}
          onBlur={handleBlur}
          InputProps={{
            ...params.InputProps,
            endAdornment: (
              <>
                {loading ? <CircularProgress color="inherit" size={20} /> : null}
                {params.InputProps.endAdornment}
              </>
            ),
          }}
        />
      )}
    />
  );
};

export default PlaceAutocomplete;