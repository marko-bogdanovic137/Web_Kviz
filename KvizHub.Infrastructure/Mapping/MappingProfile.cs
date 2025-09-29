using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KvizHub.Models.DTOs;
using KvizHub.Models.Entities;

namespace KvizHub.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<RegisterRequestDTO, User>();
            CreateMap<User, UserDTO>();
            CreateMap<User, LoginResponseDTO>();

            // Quiz mappings
            CreateMap<Quiz, QuizListDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.Questions.Count));

            CreateMap<Quiz, QuizDetailDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.Questions.Count));

            CreateMap<Question, QuestionDetailDTO>();
            CreateMap<AnswerOption, AnswerOptionDTO>();

            // Create mappings
            CreateMap<CreateQuizDTO, Quiz>();
            CreateMap<CreateQuestionDTO, Question>();
            CreateMap<CreateAnswerOptionDTO, AnswerOption>();

            // Quiz result mappings
            CreateMap<QuizResult, QuizResultDTO>()
                .ForMember(dest => dest.QuizTitle, opt => opt.MapFrom(src => src.Quiz.Title));
        }
    }
}
