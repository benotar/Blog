const initialState = {
    currentUser: null,
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
            currentUser: payload,
            loading: false,
            errorMessage: null
        }));
    },
    signInFailure: (payload) => {

        set((state) => ({
            ...state,
            currentUser: null,
            loading: false,
            errorMessage: payload
        }));
    }
});

