const initialState = {
    currentUser: null,
    tokens: null,
    errorMessage: null,
    loading: false
};

export const createUserSlice = (set) => ({
    ...initialState,
    signInStart: () => {
        set((state) => ({
            ...state,
            loading: true,
            errorMessage: null
        }));
    },
    signInSuccess: (payload) => {
        set((state) => ({
            ...state,
            currentUser: payload.currentUser,
            tokens: payload.tokens,
            loading: false,
            errorMessage: null
        }));
    },
    signInFailure: (payload) => {
        set((state) => ({
            ...state,
            currentUser: null,
            tokens: null,
            loading: false,
            errorMessage: payload
        }));
    }
});

