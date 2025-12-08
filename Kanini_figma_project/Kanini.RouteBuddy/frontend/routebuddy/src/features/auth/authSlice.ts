import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import { authAPI } from './authAPI';
import type {
  LoginRequest,
  RegisterWithOtpRequest,
  ResendOtpRequest,
  VerifyRegistrationOtpRequest,
  ForgotPasswordRequest,
  VerifyForgotPasswordOtpRequest,
  ResetPasswordRequest,
} from './types';

interface User {
  userId: number;
  email: string;
  role: string;
}

interface AuthState {
  user: User | null;
  tempEmail: string | null;
  tempRole: string | null;
  tempOtpToken: string | null;
  loading: boolean;
  error: string | null;
  isAuthenticated: boolean;
}

const loadAuthState = (): AuthState => {
  try {
    const savedUser = localStorage.getItem('user');
    if (savedUser) {
      const user = JSON.parse(savedUser);
      if (user && user.userId && user.email && user.role) {
        return {
          user,
          tempEmail: null,
          tempRole: null,
          tempOtpToken: null,
          loading: false,
          error: null,
          isAuthenticated: true,
        };
      } else {
        localStorage.removeItem('user');
      }
    }
  } catch (e) {
    console.error('Failed to load auth state', e);
    localStorage.removeItem('user');
  }
  return {
    user: null,
    tempEmail: null,
    tempRole: null,
    tempOtpToken: null,
    loading: false,
    error: null,
    isAuthenticated: false,
  };
};

const initialState: AuthState = loadAuthState();

export const registerWithOtp = createAsyncThunk(
  'auth/registerWithOtp',
  async (data: RegisterWithOtpRequest, { rejectWithValue }) => {
    try {
      const response = await authAPI.registerWithOtp(data);
      return response;
    } catch (error: any) {
      const errorMsg = error.response?.data?.error?.description || error.response?.data?.error || error.message || 'Registration failed';
      return rejectWithValue(errorMsg);
    }
  }
);

export const resendRegistrationOtp = createAsyncThunk(
  'auth/resendRegistrationOtp',
  async (data: ResendOtpRequest, { rejectWithValue }) => {
    try {
      const response = await authAPI.resendRegistrationOtp(data);
      return response;
    } catch (error: any) {
      const errorMsg = error.response?.data?.error?.description || error.response?.data?.error || error.message || 'Resend OTP failed';
      return rejectWithValue(errorMsg);
    }
  }
);

export const verifyRegistrationOtp = createAsyncThunk(
  'auth/verifyRegistrationOtp',
  async (data: VerifyRegistrationOtpRequest, { rejectWithValue }) => {
    try {
      const response = await authAPI.verifyRegistrationOtp(data);
      return response;
    } catch (error: any) {
      const errorMsg = error.response?.data?.error?.description || error.response?.data?.error || error.message || 'OTP verification failed';
      return rejectWithValue(errorMsg);
    }
  }
);

export const login = createAsyncThunk(
  'auth/login',
  async (data: LoginRequest, { rejectWithValue }) => {
    try {
      const response = await authAPI.login(data);
      return response;
    } catch (error: any) {
      const errorMsg = error.response?.data?.error?.description || error.response?.data?.error || error.message || 'Login failed';
      return rejectWithValue(errorMsg);
    }
  }
);

export const forgotPassword = createAsyncThunk(
  'auth/forgotPassword',
  async (data: ForgotPasswordRequest, { rejectWithValue }) => {
    try {
      const response = await authAPI.forgotPassword(data);
      return response;
    } catch (error: any) {
      const errorMsg = error.response?.data?.error?.description || error.response?.data?.error || error.message || 'Failed to send reset email';
      return rejectWithValue(errorMsg);
    }
  }
);

export const verifyForgotPasswordOtp = createAsyncThunk(
  'auth/verifyForgotPasswordOtp',
  async (data: VerifyForgotPasswordOtpRequest, { rejectWithValue }) => {
    try {
      const response = await authAPI.verifyForgotPasswordOtp(data);
      return response;
    } catch (error: any) {
      const errorMsg = error.response?.data?.error?.description || error.response?.data?.error || error.message || 'OTP verification failed';
      return rejectWithValue(errorMsg);
    }
  }
);

export const resetPassword = createAsyncThunk(
  'auth/resetPassword',
  async (data: ResetPasswordRequest, { rejectWithValue }) => {
    try {
      const response = await authAPI.resetPassword(data);
      return response;
    } catch (error: any) {
      const errorMsg = error.response?.data?.error?.description || error.response?.data?.error || error.message || 'Password reset failed';
      return rejectWithValue(errorMsg);
    }
  }
);



const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setTempEmail: (state, action: PayloadAction<string>) => {
      state.tempEmail = action.payload;
    },
    setTempRole: (state, action: PayloadAction<string>) => {
      state.tempRole = action.payload;
    },
    setTempOtpToken: (state, action: PayloadAction<string>) => {
      state.tempOtpToken = action.payload;
    },
    logout: (state) => {
      state.user = null;
      state.isAuthenticated = false;
      state.tempEmail = null;
      state.tempRole = null;
      localStorage.removeItem('user');
      authAPI.logout().catch(console.error);
    },
    checkAuthState: (state) => {
      const savedUser = localStorage.getItem('user');
      if (savedUser) {
        try {
          const user = JSON.parse(savedUser);
          if (user && user.userId && user.email && user.role) {
            state.user = user;
            state.isAuthenticated = true;
          } else {
            localStorage.removeItem('user');
            state.user = null;
            state.isAuthenticated = false;
          }
        } catch (e) {
          localStorage.removeItem('user');
          state.user = null;
          state.isAuthenticated = false;
        }
      } else {
        state.user = null;
        state.isAuthenticated = false;
      }
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(registerWithOtp.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(registerWithOtp.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(registerWithOtp.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(resendRegistrationOtp.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(resendRegistrationOtp.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(resendRegistrationOtp.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(verifyRegistrationOtp.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(verifyRegistrationOtp.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(verifyRegistrationOtp.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(login.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(login.fulfilled, (state, action) => {
        state.loading = false;
        const user = {
          userId: action.payload.userId,
          email: action.payload.email,
          role: action.payload.role,
        };
        state.user = user;
        state.isAuthenticated = true;
        localStorage.setItem('user', JSON.stringify(user));
      })
      .addCase(login.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(forgotPassword.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(forgotPassword.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(forgotPassword.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(verifyForgotPasswordOtp.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(verifyForgotPasswordOtp.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(verifyForgotPasswordOtp.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      .addCase(resetPassword.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(resetPassword.fulfilled, (state) => {
        state.loading = false;
      })
      .addCase(resetPassword.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const setTempUserId = (userId: number) => (dispatch: any) => {
  sessionStorage.setItem('tempUserId', userId.toString());
};

export const { setTempEmail, setTempRole, setTempOtpToken, logout, clearError, checkAuthState } = authSlice.actions;
export default authSlice.reducer;
